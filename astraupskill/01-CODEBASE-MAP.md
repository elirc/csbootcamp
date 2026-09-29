# Follow the request through three files

The [project file](../guides/crud-web-lab/CrudWebLab.csproj) targets .NET 10 with
the web SDK, nullable checking and warnings treated as errors. There are no
PackageReference entries. This companion is deliberately separate from the
bootcamp's file-based exercise runner: an HTTP application has a host lifecycle
that would distract from the original single-function exercise format.

[Program.cs](../guides/crud-web-lab/Program.cs) constructs the application,
registers one TaskStore and maps five CRUD operations. GET collection returns
an ordered snapshot. GET item returns a representation and ETag or not found.
POST validates the title, creates a task, and returns Created with Location
and an initial ETag. PUT and DELETE parse a required If-Match version before
asking the store to mutate a matching item.

[TaskStore.cs](../guides/crud-web-lab/TaskStore.cs) holds immutable TaskItem
records in a dictionary. A single lock protects dictionary access, identifier
allocation and version comparisons. Replace returns an outcome plus the new
record; Delete returns an outcome. Neither method decides HTTP status codes.
The route translates Missing, Conflict and Saved into transport responses.
This separation lets you discuss storage behavior without embedding an HTTP
dependency in every operation.

[HttpChecks.cs](../guides/crud-web-lab/HttpChecks.cs) uses HttpClient against
the running app. Its helpers create a resource, issue a conditional write and
report named cases. Each case creates its own task where needed. The app's
self-test branch starts Kestrel on an available loopback port, runs the checks,
and stops and disposes the host in a finally block. Ordinary execution listens
on the fixed local lab port instead.

Draw the path request -> binding -> title/header checks -> store -> response.
Place the lock only inside the store. Then draw a second client entering the
same store with an old version. The two arrows reveal why a lock around only
the dictionary assignment would be insufficient: comparison and mutation must
share the protected region. Keep this map beside the code when you attempt
the exercises; it is a reasoning aid, not a substitute for reading the branch
that returns each outcome.
