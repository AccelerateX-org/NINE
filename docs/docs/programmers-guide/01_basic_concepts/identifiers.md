---
sidebarposition: 1
---

# Identifying Aggregates

How objects are identified.

## Id

Returning a single object (aggregate).

UUIDs are prefered over INT

- (hopefully) global uniqueness, making it easier working with different databases in development, migration.
- not used as foreign reference across databases (of services) - context keys / adresses (data values) are used for identifying independent entities.
- performance issues are accepted, because of the small number of entities (several hundrets, 10K).
- no need for external storage (files), the context keys are used for that.

## Key

Composite key using '|' as seperator.

using the key always will return a list of objects.

## Context

The context is a container for the elements of the key.

### Module

- institution
- organisation
- catalog
- topic (inkl. version number)

Versioning: _v[x.y] where x.y as version number should use semantic versioning 

- instruction
- 

