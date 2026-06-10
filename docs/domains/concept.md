# Design guidelines

## Identifying Aggregates (independent entities)

### _id

UUIDs are prefered over INT

- (hopefully) global uniqueness, making it easier working with different databases in development, migration.
- not used as foreign reference across databases (of services) - context keys / adresses (data values) are used for identifying independent entities.
- performance issues are accepted, because of the small number of entities (several hundrets, <10K).
- no need for external storage (files), the context keys are used for that.

### context (Adress, data values)

The context is a hierarchy used as an adress.
using a context for searching is resulting in a list of entities.

### [entity]_id

The context as string using ":" as separators.



## Aggregates

Top-level aggregate: Institution, i.e. "HM" splits into Organsiers

### Actors

- Institution
- Organiser
- Member

Remark: roles like "PK", "SGL", "D", may be represented by labels like "cohorts"

### Modules (proposal)

- Institution
- Organizer
- Catalog (or field)
- Topic

2nd level (collections)

- Instruction
- Challenge
- Objective

### Modules (request)

- Institution
- Curriculum
- Amendment
- Block
- Option
- Slot
- Loading
- Chip