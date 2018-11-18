using System.Runtime.InteropServices;
using ClangSharp;

namespace SealangSharp
{
	public enum BinaryOperatorKind
	{
		// [C++ 5.5] Pointer-to-member operators.
		/// <summary>
		/// .*
		/// </summary>
		PtrMemD,

		/// <summary>
		/// ->*
		/// </summary>
		PtrMemI,

		// [C99 6.5.5] Multiplicative operators.
		Mul,
		Div,
		Rem,

		// [C99 6.5.6] Additive operators.
		Add,
		Sub,

		// [C99 6.5.7] Bitwise shift operators.
		Shl,
		Shr,

		// C++20 [expr.spaceship] Three-way comparison operator.
		ThreeWayComparison,

		// [C99 6.5.8] Relational operators.
		LT,
		GT,
		LE,
		GE,

		// [C99 6.5.9] Equality operators.
		EQ,
		NE,

		// [C99 6.5.10] Bitwise AND operator.
		And,

		// [C99 6.5.11] Bitwise XOR operator.
		Xor,

		// [C99 6.5.12] Bitwise OR operator.
		Or,

		// [C99 6.5.13] Logical AND operator.
		LAnd,

		// [C99 6.5.14] Logical OR operator.
		LOr,

		// [C99 6.5.16] Assignment operators.
		Assign,
		MulAssign,
		DivAssign,
		RemAssign,
		AddAssign,
		SubAssign,
		ShlAssign,
		ShrAssign,
		AndAssign,
		XorAssign,
		OrAssign,

		// [C99 6.5.17] Comma operator.
		Comma,
	}

	public enum UnaryOperatorKind
	{
		// [C99 6.5.2.4] Postfix increment and decrement
		PostInc,
		PostDec,
		
		// [C99 6.5.3.1] Prefix increment and decrement 
		PreInc,
		PreDec,
		
		// [C99 6.5.3.2] Address and indirection
		AddrOf,
		Deref,
		
		// [C99 6.5.3.3] Unary arithmetic 
		Plus,
		Minus,
		Not,
		LNot,
		
		// "__real expr"/"__imag expr" Extension.
		Real,
		Imag,

		// __extension__ marker.
		Extension,

		// [C++ Coroutines] co_await operator
		Coawait
	}

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
		public static extern BinaryOperatorKind cursor_getBinaryOpcode(CXCursor @cursor);

		/// <summary>
		/// Returns Opcode of unary operator
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		[DllImport(libraryPath, EntryPoint = "sealang_Cursor_getUnaryOpcode", CallingConvention = CallingConvention.Cdecl)]
		public static extern UnaryOperatorKind cursor_getUnaryOpcode(CXCursor @cursor);

		/// <summary>
		/// Returns string representation of literal cursor (1.f, 1000L, etc)
		/// </summary>
		/// <param name="cursor"></param>
		/// <returns></returns>
		[DllImport(libraryPath, EntryPoint = "sealang_Cursor_getLiteralString", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString cursor_getLiteralString(CXCursor @cursor);
	}
}