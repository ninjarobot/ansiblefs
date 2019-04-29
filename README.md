ansiblefs
=========

A custom Ansible module written in F# and compiled to native code with CoreRT.

---------

Ansible is a mature platform for deployment and configuration of software and infrastructure with a vibrant ecosystem of
extensions and modules. A module is a unit of code that perform idempotent operations by being copied to a remote system
where it will be executed by passing JSON to the standard input. The module will then return JSON by writing it to
standard output.

### Why F#?

With the rich ecosystem for Ansible, it's best to use an existing module, and most of those are written in Python. There
are packages available to simplify the development of a module when writing it in Python, and if you can, you probably 
should make your module in Python.  However, there are some very good reasons to choose another language.

#### API support
The software or infrastructure being installed may not provide a Python API, but provides API's and libraries for the
Common Language Runtime.

#### Language Features
The F# language has strong type safety, a powerful standard library, and concurrency primitives, among many other 
features that can be used with minimal code complexity. These features are called out here because they are useful in
the context of gathering facts and performing deployment and configuration. For example, if a playbook to install an
application requires performing dozens of API calls to get information about the system or environment, making many
calls concurrently and correlating the results is typically a simple task in F#.

### Why CoreRT

It's helpful to keep the module dependencies to a minimum. If a module requires an entire runtime to be installed, the
module will take a longer time to run and have a fairly large footprint on the target system due to the runtime. That
runtime may also not be desired on the target system, so it would need to be fully removed. To avoid this complexity, 
the example herein uses CoreRT to compile the F# code to a small, self-contained, native executable.

Prerequisites
-------------

Ansible 2.2 or greater is required in order to suppport binary modules.

On Linux, CoreRT compilation requires `clang-3.9`, `zlib1g-dev`, and `libkrb5-dev`.
