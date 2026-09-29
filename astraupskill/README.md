# From C# fundamentals to a versioned CRUD API

The existing bootcamp teaches language fundamentals through small offline
exercises. This companion lab connects those skills to an actual HTTP service.
It lives in [guides/crud-web-lab](../guides/crud-web-lab/Program.cs), outside the
numbered exercise modules, so it does not change their authoring format or
their scoreboard. The lab uses the .NET 10 shared framework and no third-party
package references. You can study it after the collections and methods modules.

The resource is a task with an identifier, title, completion flag and version.
You can create, list, read, replace and delete tasks. The interesting rule is
that a replacement or deletion must name the version the client observed.
This lets you reproduce a common CRUD failure: two browser tabs read the same
record, then the second save silently overwrites the first. The corrected
store compares and changes the version in one locked section.

Start with the concepts chapter and predict the two-editor result. Then read
the route map, follow the worked example and run the HTTP checks. They start
a real local server on an ephemeral loopback port, send requests, inspect
responses and stop the server. They are not merely assertions against copied
service logic. Read the verification chapter before interpreting their scope.

Practice uses several methods: a request trace table, a deliberately proposed
broken implementation for review, small mutation experiments, and a database
design discussion. The answer chapter is separate so you can explain your
prediction before checking it. Keep a notebook of expected state, actual
state and the assertion that distinguishes them.

This is a single-process learning service with no authentication and no durable
storage. Restarting loses tasks. It is not a production starter or a complete
HTTP conditional-request implementation. Those limits are useful teaching
boundaries: they let you isolate validation and lost-update behavior before
adding identity, persistence and deployment. A strong completion explanation
names both the guarantee demonstrated here and the stronger guarantee a real
database application would need.
