# Set up the governance

## Institution

Institutions are representing a school or university.

| Attribute | Description
|-----|----|
| shortName | used as key, i.e. "HM" |
| name | "Hochschule München" |
| img | logo |

Every institution is divided into one or more departments.

## Department

Departments are responsible for offering modules, curricula and courses.

| Attribute | Description
|-----|----|
| shortName | used as key, i.e. "FK 09" |
| name | "Fakultät für Wirtschaftsingenieurwesen" |

Every department has members.

## Member

Members are representing a role of a person (user account) with a department.

| Role | Description
|-----|----|
| *none* | just beeing member, creating courses and modules |
| person admin | managing members including assigning user accounts |
| room admin | booking rooms marked as *private* |
| course admin | changing courses, planing semester |
| exam admin | planing semester |
| student admin | cohorts and enrollment |
| org admin | managing roles |

:::info
A single person (user account) can be assigned to more than one department. 
:::

