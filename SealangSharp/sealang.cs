using System.Runtime.InteropServices;
using ClangSharp;

namespace SealangSharp
{
	public static class sealang
	{
		public const string libraryPath = "sealang";

		/// <summary>
		/// Returns string representation of unary and binary operators
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		[DllImport(libraryPath, EntryPoint = "sealang_Cursor_getOperatorString", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString cursor_getOperatorString(CXCursor @cursor);

		/// <summary>
		/// Returns Opcode of binary operator
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		[DllImport(libraryPath, EntryPoint = "sealang_Cursor_getBinaryOpcode", CallingConvention = CallingConvention.Cdecl)]
		public static extern int cursor_getBinaryOpcode(CXCursor @cursor);

		/// <summary>
		/// Returns Opcode of unary operator
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		[DllImport(libraryPath, EntryPoint = "sealang_Cursor_getUnaryOpcode", CallingConvention = CallingConvention.Cdecl)]
		public static extern int cursor_getUnaryOpcode(CXCursor @cursor);

		/// <summary>
		/// Returns string representation of literal cursor (1.f, 1000L, etc)
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		[DllImport(libraryPath, EntryPoint = "sealang_Cursor_getLiteralString", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString cursor_getLiteralString(CXCursor @cursor);
	}
}
