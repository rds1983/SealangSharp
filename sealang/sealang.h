#ifndef SEALANG_H
#define SEALANG_H

#include "clang/AST/OperationKinds.h"
#include "clang-c/Index.h"
#include "clang-c/CXString.h"

#ifdef __cplusplus
extern "C" {
#endif

/* MSVC DLL import/export. */
#ifdef _MSC_VER
#ifdef _SEALANG_LIB_
#define SEALANG_LINKAGE __declspec(dllexport)
#else
#define SEALANG_LINKAGE __declspec(dllimport)
#endif
#else
#define SEALANG_LINKAGE
#endif

/**
 * \brief Returns string representation of unary and binary operators
 */
SEALANG_LINKAGE CXString sealang_Cursor_getOperatorString(CXCursor cursor);

/**
 * \brief Returns Opcode of binary operator
 */
SEALANG_LINKAGE clang::BinaryOperatorKind sealang_Cursor_getBinaryOpcode(CXCursor cursor);

/**
 * \brief Returns Opcode of unary operator
 */
SEALANG_LINKAGE clang::UnaryOperatorKind sealang_Cursor_getUnaryOpcode(CXCursor cursor);

/**
 * \brief Returns string representation of literal cursor (1.f, 1000L, etc)
 */
SEALANG_LINKAGE CXString sealang_Cursor_getLiteralString(CXCursor cursor);

/**
 * \brief Returns for-loop init cursor [for(init;cond;inc)], or CXCursor_NoDeclFound if there is no decl,
 * or CXCursor_InvalidCode if C is not CXCursor_ForStmt
 */
// CXCursor clang_getForStmtInit(CXCursor C);

/**
 * \brief Returns for-loop condition cursor [for(init;cond;inc)], or CXCursor_NoDeclFound if there is no decl,
 * or CXCursor_InvalidCode if C is not CXCursor_ForStmt
 */
// CXCursor clang_getForStmtCond(CXCursor C);

/**
 * \brief Returns for-loop increment cursor [for(init;cond;inc)], or CXCursor_NoDeclFound if there is no decl,
 * or CXCursor_InvalidCode if C is not CXCursor_ForStmt
 */
// CXCursor clang_getForStmtInc(CXCursor C);

/**
 * \brief Returns for-loop body, or CXCursor_NoDeclFound if there is no decl,
 * or CXCursor_InvalidCode if C is not CXCursor_ForStmt
 */
// CXCursor clang_getForStmtBody(CXCursor C);


#ifdef __cplusplus
}
#endif

#endif
