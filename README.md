# Outcome

Outcome is a small personal project that you may find useful, but you should remember this is meant to be a small personal project ;).

## Overview

This project provides or extends the following useful computation expressions for F#:

1. Result - works directly with the Result type
1. Async - extends the Async type
1. AsyncResult - combines the async and result computation expressions
1. Validation - handles the applicative flows for the result with minimal mapErrors
1. ValidationAsync - combines the validation and async computation expression

### Results

The result computation expression focuses on making it easier to use the Result type with Railway oriente programming in mind. It supports both the an applicative and monadic usage when working with an error path, but always requires the error types to be explicitly mapped.

### Validation

The validation computation expression is designed to automatically map errors to a common type. All the original type information is preserved an can be accessed though the `ErrorList` type. The general idea here is that an error is an error and it generally just gets mapped to a message.

Now in practice this does not always hold. We have errors that need to be handle differently form others. For example in a web api validation from the user may need to be mapped to a 422, but the same does not hold for validation errors triggered by data from the database. We leverage the `ErrorList` type to help here. We can filter it down to specific errors to ignore others or we can conditional map on only certain errors.
