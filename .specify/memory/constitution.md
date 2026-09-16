# SmartSpace Constitution

## Core Principles

### I. Traceable Business Behavior
Every business requirement MUST map to at least one user scenario, acceptance
criterion, and automated test. Backlog items and implementation tasks MUST retain
that traceability so that scope decisions can be reviewed without inferring intent
from code. This makes the prototype auditable and keeps business behavior separate
from incidental technical design.

### II. SQL Server as Persistent Source of Truth
SQL Server MUST be the persistent source of truth for durable SmartSpace data.
Booking integrity MUST hold across concurrent requests and multiple API processes,
using database-enforced or transactionally equivalent guarantees. A local SQLite
in-memory profile MAY exist as a bounded training aid, but MUST NOT be presented as
the durable system or as evidence of production-grade persistence and concurrency.

### III. Backend-Enforced Authorization and Ownership
The backend MUST derive identity and ownership from server-validated authentication
claims. Every protected endpoint and mutation MUST enforce authorization server-side;
UI visibility is not an authorization boundary. A user MUST be unable to read,
change, or cancel another user's reservation unless an explicitly approved policy
grants that capability.

### IV. Development Identity Isolation
Development-only identities and authentication handlers MUST be disabled outside
Development. Startup MUST fail when demo authentication is enabled in another
environment. Production-like verification MUST use the configured real identity
boundary rather than a selectable username, request header, or client-supplied ID.

### V. Preserved History and No Booking Hard Delete
Reservation history MUST remain queryable according to the approved ownership and
privacy policy. Reservation cancellation MUST preserve the reservation record and
release its time interval. The release MUST NOT hard-delete reservations or rooms
referenced by reservations; deactivation MUST be used when a room is no longer
bookable.

### VI. Approved Stack and Simplest Suitable Architecture
The project MUST use the documented SmartSpace stack unless an amendment records a
replacement: .NET 10, standalone Blazor WebAssembly, ASP.NET Core Minimal API,
Entity Framework Core, shared C# contracts, Tailwind CSS, and SQL Server for
persistent storage. The solution MUST remain a single application with clear
feature boundaries unless scale or an approved requirement proves a simpler
architecture insufficient. Microservices, a generic repository layer, and
mandatory CQRS/MediatR MUST NOT be introduced without a documented rationale.

### VII. Feature-Level Failure and Automated Test Coverage
Every feature MUST define expected validation, authorization, conflict, persistence,
and dependency-failure behavior where applicable. Relevant automated tests MUST
cover both the success path and its meaningful failure scenarios. Tests for booking
integrity MUST exercise the real persistence and transaction boundary rather than
relying only on mocked repositories.

### VIII. Secret and Personal Data Protection
Secrets MUST NOT be committed to source code, configuration checked into the
repository, test fixtures, or logs. Request logs MUST NOT contain unnecessary
personal data, authentication tokens, or reservation-owner details. Diagnostic data
MUST be minimized, access-controlled, and reviewed when a new endpoint or log field
is introduced.

### IX. Keyboard-Accessible and Understandable UI
Every user workflow MUST be operable with a keyboard and MUST expose visible labels,
focus states, and text alternatives for status communicated by color. Validation,
authorization, conflict, and network failures MUST produce concise, understandable
messages in the product language. Responsive behavior MUST preserve access to the
same supported actions without horizontal overflow at the documented viewport sizes.

### X. Proposal Status Is Explicit
New assumptions, policies, architecture choices, and UI behaviors MUST be marked as
proposals until the responsible BIDN stakeholders confirm them. Documentation MUST
distinguish original business input, lab proposals, and approved decisions. No
implementation task MAY silently convert a proposal into a product requirement.

## Project Constraints

Release 1 covers meeting-room discovery, availability, reservation, modification,
cancellation, personal history, and room administration. Workspaces as a user
feature, recurring bookings, notifications, participant lists, check-in/check-out,
Outlook, Teams, Microsoft Graph, reporting, and capacity analysis are outside this
release. A future-ready data model MUST NOT be treated as permission to build those
features now.

The local prototype uses synthetic data and is not production acceptance. The
prototype MUST state its persistence, identity, hosting, and concurrency limits.
The intended production identity, hosting, backup/recovery, and SQL Server
configuration require explicit BIDN decisions before a production claim is made.

The referenced `StakeholderDocuments/OpenQuestions.md` was not present when this
constitution was created. Until that source is restored or replaced, unresolved
decisions in `StakeholderDocuments/TechStack.md` §15 remain open and MUST NOT be
treated as approvals.

## Development Workflow and Quality Gates

Before implementation, the team MUST validate scope, authorization policy,
time-zone behavior, booking horizon, buffers, administrative powers, persistent
storage, identity integration, and API contracts with the responsible stakeholders.

Each backlog item MUST identify its requirement, business rule, scenario, API/UI
contract, failure cases, and tests. A feature is complete only when the relevant
automated tests pass, authorization and ownership checks are demonstrated, SQL
Server concurrency behavior is verified where relevant, and accessibility checks
cover keyboard operation and understandable errors.

Changes MUST receive review for requirement traceability, security and privacy,
data integrity, accessibility, test evidence, and adherence to this constitution.
Training-demo evidence MUST be labeled as such and MUST NOT be used as production
acceptance evidence without the required environment-specific validation.

## Governance

This constitution governs SmartSpace requirements, design, implementation,
verification, and review. When another document conflicts with it, the conflict MUST
be recorded and resolved before the affected work proceeds. A proposal or generated
artifact cannot override a principle by implication.

Amendments MUST be proposed in a reviewable change to this file. The proposal MUST
state the motivation, affected principles, compatibility impact, required updates
to templates or guidance, and any migration or validation work. The responsible
product and technical reviewers MUST approve the amendment before it is treated as
effective. An amendment MUST NOT include unrelated application implementation.

Constitution versions use semantic versioning:

- MAJOR increments for removing or redefining a principle in a backward-incompatible
	way.
- MINOR increments for adding a principle or materially expanding governance,
	constraints, or required quality gates.
- PATCH increments for clarifications, wording corrections, and non-semantic
	refinements.

Every amendment MUST update the version and last-amended date, preserve a concise
change rationale in review history, and trigger a compliance review of affected
specifications, plans, tasks, tests, and documentation. Compliance review MUST
confirm that no proposal is represented as an approved requirement and that all
mandatory gates remain testable.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): awaiting formal BIDN adoption | **Last Amended**: 2026-09-16
