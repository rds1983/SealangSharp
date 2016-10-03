using System;
using System.IO;
using System.Text;
using ClangSharp;
using SealangSharp;

namespace SealangSharpTest
{
	internal static class Program
	{
		private static TextWriter _writer;
		private static int _indentLevel = 2;

		static void Main(string[] args)
		{
			var createIndex = clang.createIndex(0, 0);
			CXUnsavedFile unsavedFile;

			CXTranslationUnit tu;

			var arr = new string[0];

			var res = clang.parseTranslationUnit2(createIndex,
				@"main.c",
				arr,
				arr.Length,
				out unsavedFile,
				0,
				0,
				out tu);

			if (res != CXErrorCode.CXError_Success)
			{
				var sb = new StringBuilder();

				sb.AppendLine(res.ToString());

				var numDiagnostics = clang.getNumDiagnostics(tu);
				for (uint i = 0; i < numDiagnostics; ++i)
				{
					var diag = clang.getDiagnostic(tu, i);
					sb.AppendLine(clang.getDiagnosticSpelling(diag).ToString());
					clang.disposeDiagnostic(diag);
				}

				throw new Exception(sb.ToString());
			}

			var executableLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
			var executableFolder = Path.GetDirectoryName(executableLocation);

			var outputPath = Path.Combine(executableFolder, @"output");
			using (_writer = new StreamWriter(outputPath))
			{
				var data = new CXClientData((IntPtr) 0);
				clang.visitChildren(clang.getTranslationUnitCursor(tu), Visit, data);
			}

			clang.disposeTranslationUnit(tu);
			clang.disposeIndex(createIndex);
		}

		static void WriteIndent()
		{
			for (var i = 0; i < _indentLevel; ++i)
			{
				_writer.Write("\t");
			}
		}

		static void IndentedWriteLine(string line)
		{
			WriteIndent();
			_writer.WriteLine(line);
		}

		static CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
		{
			var cursorKind = clang.getCursorKind(cursor);

			var line = string.Format("// {0}- {1} - {2}", clang.getCursorKindSpelling(cursorKind),
				clang.getCursorSpelling(cursor),
				clang.getTypeSpelling(clang.getCursorType(cursor)));

			var addition = string.Empty;

			switch (cursorKind)
			{
				case CXCursorKind.CXCursor_UnaryOperator:
					addition = string.Format("Unary Operator: {0} ({1})",
						sealang.cursor_getUnaryOpcode(cursor),
						sealang.cursor_getOperatorString(cursor));
					break;
				case CXCursorKind.CXCursor_BinaryOperator:
					addition = string.Format("Binary Operator: {0} ({1})",
						sealang.cursor_getBinaryOpcode(cursor),
						sealang.cursor_getOperatorString(cursor));
					break;
				case CXCursorKind.CXCursor_IntegerLiteral:
				case CXCursorKind.CXCursor_FloatingLiteral:
				case CXCursorKind.CXCursor_CharacterLiteral:
				case CXCursorKind.CXCursor_StringLiteral:
					addition = string.Format("Literal: {0}",
						sealang.cursor_getLiteralString(cursor));
					break;
			}

			if (!string.IsNullOrEmpty(addition))
			{
				line += " [" + addition + "]";
			}

			IndentedWriteLine(line);

			_indentLevel++;
			clang.visitChildren(cursor, Visit, new CXClientData(IntPtr.Zero));
			_indentLevel--;

			return CXChildVisitResult.CXChildVisit_Continue;
		}
	}
}
