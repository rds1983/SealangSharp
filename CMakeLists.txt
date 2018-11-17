cmake_minimum_required (VERSION 2.8)
project (sealang)

# llvm
set(LIBCLANG_BUILD_STATIC ON CACHE BOOL
  "Build libclang as a static library (in addition to a shared one)")
set(CLANG_BUILD_TOOLS OFF CACHE BOOL
	"Build the Clang tools. If OFF, just generate build targets.")
set(CLANG_INCLUDE_DOCS OFF CACHE BOOL
	"Generate build targets for the Clang docs.")
set(LLVM_BUILD_UTILS OFF CACHE BOOL
	"Build LLVM utility binaries. If OFF, just generate build targets.")
set(LLVM_INCLUDE_DOCS OFF CACHE BOOL
	"Generate build targets for llvm documentation.")
set(LLVM_INCLUDE_EXAMPLES OFF CACHE BOOL
	"Generate build targets for the LLVM examples")
set(LLVM_INCLUDE_GO_TESTS OFF CACHE BOOL
	"Include the Go bindings tests in test build targets.")
set(LLVM_INCLUDE_TESTS OFF CACHE BOOL
	"Generate build targets for the LLVM unit tests.")
set(LLVM_INCLUDE_UTILS OFF CACHE BOOL
	"Generate build targets for the LLVM utils.")
set(LLVM_ENABLE_OCAMLDOC OFF CACHE BOOL
	"Build OCaml bindings documentation.")

add_subdirectory ("llvm")

# sealang
include_directories("llvm/include")
include_directories("${CMAKE_CURRENT_BINARY_DIR}/llvm/include")
include_directories("llvm/tools/clang/include")
include_directories("${CMAKE_CURRENT_BINARY_DIR}/llvm/tools/clang/include")
add_subdirectory("sealang")