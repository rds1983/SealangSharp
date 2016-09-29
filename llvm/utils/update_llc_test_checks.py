#!/usr/bin/env python2.7

"""A test case update script.

This script is a utility to update LLVM X86 'llc' based test cases with new
FileCheck patterns. It can either update all of the tests in the file or
a single test function.
"""

import argparse
import itertools
import os         # Used to advertise this file's name ("autogenerated_note").
import string
import subprocess
import sys
import tempfile
import re

# Invoke the tool that is being tested.
def llc(args, cmd_args, ir):
  with open(ir) as ir_file:
    stdout = subprocess.check_output(args.llc_binary + ' ' + cmd_args,
                                     shell=True, stdin=ir_file)
  # Fix line endings to unix CR style.
  stdout = stdout.replace('\r\n', '\n')
  return stdout


# RegEx: this is where the magic happens.

SCRUB_WHITESPACE_RE = re.compile(r'(?!^(|  \w))[ \t]+', flags=re.M)
SCRUB_TRAILING_WHITESPACE_RE = re.compile(r'[ \t]+$', flags=re.M)
SCRUB_X86_SHUFFLES_RE = (
    re.compile(
        r'^(\s*\w+) [^#\n]+#+ ((?:[xyz]mm\d+|mem)( \{%k\d+\}( \{z\})?)? = .*)$',
        flags=re.M))
SCRUB_X86_SP_RE = re.compile(r'\d+\(%(esp|rsp)\)')
SCRUB_X86_RIP_RE = re.compile(r'[.\w]+\(%rip\)')
SCRUB_X86_LCP_RE = re.compile(r'\.LCPI[0-9]+_[0-9]+')
SCRUB_KILL_COMMENT_RE = re.compile(r'^ *#+ +kill:.*\n')

RUN_LINE_RE = re.compile('^\s*;\s*RUN:\s*(.*)$')
IR_FUNCTION_RE = re.compile('^\s*define\s+(?:internal\s+)?[^@]*@(\w+)\s*\(')
ASM_FUNCTION_RE = re.compile(
    r'^_?(?P<func>[^:]+):[ \t]*#+[ \t]*@(?P=func)\n[^:]*?'
    r'(?P<body>^##?[ \t]+[^:]+:.*?)\s*'
    r'^\s*(?:[^:\n]+?:\s*\n\s*\.size|\.cfi_endproc|\.globl|\.comm|\.(?:sub)?section)',
    flags=(re.M | re.S))
CHECK_PREFIX_RE = re.compile('--check-prefix=(\S+)')
CHECK_RE = re.compile(r'^\s*;\s*([^:]+?)(?:-NEXT|-NOT|-DAG|-LABEL)?:')


def scrub_asm(asm):
  # Scrub runs of whitespace out of the assembly, but leave the leading
  # whitespace in place.
  asm = SCRUB_WHITESPACE_RE.sub(r' ', asm)
  # Expand the tabs used for indentation.
  asm = string.expandtabs(asm, 2)
  # Detect shuffle asm comments and hide the operands in favor of the comments.
  asm = SCRUB_X86_SHUFFLES_RE.sub(r'\1 {{.*#+}} \2', asm)
  # Generically match the stack offset of a memory operand.
  asm = SCRUB_X86_SP_RE.sub(r'{{[0-9]+}}(%\1)', asm)
  # Generically match a RIP-relative memory operand.
  asm = SCRUB_X86_RIP_RE.sub(r'{{.*}}(%rip)', asm)
  # Generically match a LCP symbol.
  asm = SCRUB_X86_LCP_RE.sub(r'{{\.LCPI.*}}', asm)
  # Strip kill operands inserted into the asm.
  asm = SCRUB_KILL_COMMENT_RE.sub('', asm)
  # Strip trailing whitespace.
  asm = SCRUB_TRAILING_WHITESPACE_RE.sub(r'', asm)
  return asm


# Build up a dictionary of all the function bodies.
def build_function_body_dictionary(raw_tool_output, prefixes, func_dict, verbose):
  for m in ASM_FUNCTION_RE.finditer(raw_tool_output):
    if not m:
      continue
    func = m.group('func')
    scrubbed_body = scrub_asm(m.group('body'))
    if func.startswith('stress'):
      # We only use the last line of the function body for stress tests.
      scrubbed_body = '\n'.join(scrubbed_body.splitlines()[-1:])
    if verbose:
      print >>sys.stderr, 'Processing function: ' + func
      for l in scrubbed_body.splitlines():
        print >>sys.stderr, '  ' + l
    for prefix in prefixes:
      if func in func_dict[prefix] and func_dict[prefix][func] != scrubbed_body:
        if prefix == prefixes[-1]:
          print >>sys.stderr, ('WARNING: Found conflicting asm under the '
                               'same prefix: %r!' % (prefix,))
        else:
          func_dict[prefix][func] = None
          continue

      func_dict[prefix][func] = scrubbed_body


def add_checks(output_lines, prefix_list, func_dict, func_name):
  printed_prefixes = []
  for checkprefixes, _ in prefix_list:
    for checkprefix in checkprefixes:
      if checkprefix in printed_prefixes:
        break
      if not func_dict[checkprefix][func_name]:
        continue
      # Add some space between different check prefixes.
      if len(printed_prefixes) != 0:
        output_lines.append(';')
      printed_prefixes.append(checkprefix)
      output_lines.append('; %s-LABEL: %s:' % (checkprefix, func_name))
      func_body = func_dict[checkprefix][func_name].splitlines()
      output_lines.append('; %s:       %s' % (checkprefix, func_body[0]))
      for func_line in func_body[1:]:
        output_lines.append('; %s-NEXT:  %s' % (checkprefix, func_line))
      # Add space between different check prefixes and the first line of code.
      # output_lines.append(';')
      break
  return output_lines


def should_add_line_to_output(input_line, prefix_set):
  # Skip any blank comment lines in the IR.
  if input_line.strip() == ';':
    return False
  # Skip any blank lines in the IR.
  #if input_line.strip() == '':
  #  return False
  # And skip any CHECK lines. We're building our own.
  m = CHECK_RE.match(input_line)
  if m and m.group(1) in prefix_set:
    return False

  return True


def main():
  parser = argparse.ArgumentParser(description=__doc__)
  parser.add_argument('-v', '--verbose', action='store_true',
                      help='Show verbose output')
  parser.add_argument('--llc-binary', default='llc',
                      help='The "llc" binary to use to generate the test case')
  parser.add_argument(
      '--function', help='The function in the test file to update')
  parser.add_argument('tests', nargs='+')
  args = parser.parse_args()

  autogenerated_note = ('; NOTE: Assertions have been autogenerated by '
                        'utils/' + os.path.basename(__file__))

  for test in args.tests:
    if args.verbose:
      print >>sys.stderr, 'Scanning for RUN lines in test file: %s' % (test,)
    with open(test) as f:
      input_lines = [l.rstrip() for l in f]

    run_lines = [m.group(1)
                 for m in [RUN_LINE_RE.match(l) for l in input_lines] if m]
    if args.verbose:
      print >>sys.stderr, 'Found %d RUN lines:' % (len(run_lines),)
      for l in run_lines:
        print >>sys.stderr, '  RUN: ' + l

    prefix_list = []
    for l in run_lines:
      (llc_cmd, filecheck_cmd) = tuple([cmd.strip() for cmd in l.split('|', 1)])
      if not llc_cmd.startswith('llc '):
        print >>sys.stderr, 'WARNING: Skipping non-llc RUN line: ' + l
        continue

      if not filecheck_cmd.startswith('FileCheck '):
        print >>sys.stderr, 'WARNING: Skipping non-FileChecked RUN line: ' + l
        continue

      llc_cmd_args = llc_cmd[len('llc'):].strip()
      llc_cmd_args = llc_cmd_args.replace('< %s', '').replace('%s', '').strip()

      check_prefixes = [m.group(1)
                        for m in CHECK_PREFIX_RE.finditer(filecheck_cmd)]
      if not check_prefixes:
        check_prefixes = ['CHECK']

      # FIXME: We should use multiple check prefixes to common check lines. For
      # now, we just ignore all but the last.
      prefix_list.append((check_prefixes, llc_cmd_args))

    func_dict = {}
    for prefixes, _ in prefix_list:
      for prefix in prefixes:
        func_dict.update({prefix: dict()})
    for prefixes, llc_args in prefix_list:
      if args.verbose:
        print >>sys.stderr, 'Extracted LLC cmd: llc ' + llc_args
        print >>sys.stderr, 'Extracted FileCheck prefixes: ' + str(prefixes)

      raw_tool_output = llc(args, llc_args, test)
      build_function_body_dictionary(raw_tool_output, prefixes, func_dict, args.verbose)

    is_in_function = False
    is_in_function_start = False
    prefix_set = set([prefix for prefixes, _ in prefix_list for prefix in prefixes])
    if args.verbose:
      print >>sys.stderr, 'Rewriting FileCheck prefixes: %s' % (prefix_set,)
    output_lines = []
    output_lines.append(autogenerated_note)

    for input_line in input_lines:
      if is_in_function_start:
        if input_line == '':
          continue
        if input_line.lstrip().startswith(';'):
          m = CHECK_RE.match(input_line)
          if not m or m.group(1) not in prefix_set:
            output_lines.append(input_line)
            continue

        # Print out the various check lines here.
        output_lines = add_checks(output_lines, prefix_list, func_dict, name)
        is_in_function_start = False

      if is_in_function:
        if should_add_line_to_output(input_line, prefix_set) == True:
          # This input line of the function body will go as-is into the output.
          output_lines.append(input_line)
        else:
          continue
        if input_line.strip() == '}':
          is_in_function = False
        continue

      if input_line == autogenerated_note:
        continue

      # If it's outside a function, it just gets copied to the output.
      output_lines.append(input_line)

      m = IR_FUNCTION_RE.match(input_line)
      if not m:
        continue
      name = m.group(1)
      if args.function is not None and name != args.function:
        # When filtering on a specific function, skip all others.
        continue
      is_in_function = is_in_function_start = True

    if args.verbose:
      print>>sys.stderr, 'Writing %d lines to %s...' % (len(output_lines), test)

    with open(test, 'wb') as f:
      f.writelines([l + '\n' for l in output_lines])


if __name__ == '__main__':
  main()
