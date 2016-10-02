using System;
using System.IO;
using System.Text;
using ClangSharp;
using SealangSharp;

namespace SealangSharpTest
{
	static class Program
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

			if (cursorKind == CXCursorKind.CXCursor_BinaryOperator)
			{
				var opCode = sealang.cursor_getBinaryOpcode(cursor);

				var k = 5;
			}

			IndentedWriteLine(string.Format("// {0}- {1} - {2}", clang.getCursorKindSpelling(cursorKind),
				clang.getCursorSpelling(cursor),
				clang.getTypeSpelling(clang.getCursorType(cursor))));

			_indentLevel++;
			clang.visitChildren(cursor, Visit, new CXClientData(IntPtr.Zero));
			_indentLevel--;

			return CXChildVisitResult.CXChildVisit_Continue;
		}
	}
}
