cmake_minimum_required (VERSION 2.8)
project (sealang)

# CMake Customization
set (LIBRARY_OUTPUT_PATH ${CMAKE_SOURCE_DIR}/output)
set (EXECUTABLE_OUTPUT_PATH ${CMAKE_SOURCE_DIR}/output)

# Dependencies

# llvm
set(LIBCLANG_BUILD_STATIC ON CACHE BOOL
  "Build libclang as a static library (in addition to a shared one)")

add_subdirectory ("llvm")

# sealang
include_directories ("llvm/include")
include_directories ("llvm/tools/clang/include")
add_subdirectory ("sealang")
