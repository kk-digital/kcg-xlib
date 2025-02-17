# README for lib-hash Library

This document provides an overview and detailed documentation for the `lib-hash` library.

## Contents

- [Overview](#overview)
- [Documentation](#documentation)
  - [Hash Class](#hash-class)
    - [Constructors](#constructors)
    - [Static Methods](#static-methods)
    - [Instance Methods](#instance-methods)

## Overview

The `lib-hash` library contains a single class, `Hash`, which is used for computing and handling BLAKE2s hash values. It provides functionality to create hash values from files, byte arrays, hexadecimal strings, and integers, and also offers methods to retrieve these hashes in various formats such as hexadecimal strings, integers, and raw binary data.

The class leverages the `Blake2Fast` library for performing the actual hash calculations.

## Documentation

### Hash Class

The `Hash` class is used for representing BLAKE2s hash values. Below are the constructors and methods provided by this class.

#### Constructors

- **Hash(string filePath)**
  - Initializes a new instance of the `Hash` class by computing the hash value of a file specified by its path.

- **private Hash(byte[] byteArray)**
  - Initializes a new instance of the `Hash` class from a byte array.

#### Static Methods

- **public static async Hash Create(string filePath)**
  - Initializes a new instance of the `Hash` class by computing the hash value of a file specified by its path.

- **public static Hash FromByteArray(byte[] byteArray)**
  - Creates a new instance of the `Hash` class from a byte array.

- **public static Hash FromInt(int value)**
  - Recreates a new `Hash` object from an integer value.

- **public static Hash FromString(string hexStr)**
  - Recreates a new `Hash` object from a hexadecimal string.