# Component 2: Events Web App

## Project Description

The **Events Web App** is a multi-module application that focuses on day-to-day event
operations. Users can create, update, and manage events, assign venues and staff, track
guest bookings, and review event details in one place. The UI emphasizes clear lists and
detail views so organizers can quickly check dates, reservation references, and staffing
coverage without jumping between pages.

The solution also integrates with related services in the wider system, such as venue
availability/reservation APIs and catering data. This keeps event management flexible and
closer to real operational workflows, where different teams own different data sources.

From a technical perspective, the project uses Razor Pages with Entity Framework Core for
data access. The design emphasizes clean separation of concerns (page models for logic,
views for rendering), consistent validation, and readable code to support ongoing
maintenance and extension.

This project was developed to meet the assignment requirements while keeping the code
organized, user-friendly, and easy to extend with new features such as extra reporting or
role-based staffing checks.

## Key Problems, Challenges, and Alternative Solutions

### 1) Coordinating data across services (Events, Venues, Catering)
**Challenge:** The app needs to show venue details and reservations in the Events UI while the data lives in a separate service/database. This adds complexity when displaying details (name, description, capacity) and when releasing reservations on cancel.

**Primary approach:** Use the existing Venues API endpoints from the Events service and reuse reservation/availability data to populate venue details.

**Alternative solutions:**
- **Shared database access:** Allow the Events service to connect directly to the Venues database (not recommended in a microservice architecture because it tightly couples services).
- **Data duplication:** Store venue metadata in the Events database to avoid cross-service calls (simplifies reads but introduces data synchronization concerns).
- **Event-driven sync:** Publish venue updates to a message bus and keep a cached copy in Events (more robust but adds infrastructure).

### 2) Soft delete vs. hard delete for events
**Challenge:** Cancelling an event should preserve historical data while freeing resources (venue reservations and staff assignments). It also needs to avoid breaking existing screens that query event data.

**Primary approach:** Soft delete events by marking them cancelled and removing associated staff assignments; hide cancelled events in lists/details.

**Alternative solutions:**
- **Hard delete:** Remove records entirely (simpler but loses audit/history).
- **Separate status table:** Track cancellation in a separate table and join on queries (more normalization but adds query complexity).
- **Archival database:** Move cancelled events to an archive DB (keeps active tables small but adds cross-database reporting overhead).

### 3) Schema drift (missing columns after model changes)
**Challenge:** Adding new model properties (like `IsCancelled`) can break existing SQLite databases with errors such as `no such column`. Team members may also run different schema versions locally.

**Primary approach:** Apply EF Core migrations during app startup (`Database.Migrate()`), ensuring the DB schema matches the current model.

**Alternative solutions:**
- **Manual SQL patch:** Execute `ALTER TABLE` to add new columns when migrations cannot run (fast but must be tracked and documented).
- **Database reset:** Delete and recreate local DBs when data preservation is not required (simple but destructive).

### 4) Staff coverage visibility
**Challenge:** Event organizers need a quick view of upcoming events without staff or required roles.

**Primary approach:** Add a secondary list that filters upcoming events with no staff and sorts by date.

**Alternative solutions:**
- **Role-based validation:** Enforce required roles at booking time and block event creation/confirmation.
- **Dashboard widgets:** Create a dedicated staffing dashboard with role coverage indicators.

### References
- Microsoft Docs: Entity Framework Core overview — https://learn.microsoft.com/ef/core/
- Microsoft Docs: EF Core migrations — https://learn.microsoft.com/ef/core/managing-schemas/migrations/
- Microsoft Docs: Applying migrations (`Database.Migrate()`) — https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying
- Microsoft Docs: ASP.NET Core Razor Pages — https://learn.microsoft.com/aspnet/core/razor-pages/
- Microsoft Docs: ASP.NET Core Web API overview — https://learn.microsoft.com/aspnet/core/web-api/
- Microsoft Docs: Architectural styles (microservices) — https://learn.microsoft.com/dotnet/architecture/microservices/
- Microsoft Docs: Background tasks and event-driven processing — https://learn.microsoft.com/aspnet/core/fundamentals/host/hosted-services

## Installation Instructions
## Clone the Repository**:
   git clone https://github.com/Ali-Shehdy/Apex


  ### MUST Requirements (Completed)
- **Web Services (Apex.Catering) to create, edit, delete, and list food items:** Implemented full CRUD endpoints for food items in line with the ERD, tested with sample data to verify create/read/update/delete flows.
- **Web Services (Apex.Catering) to create, edit, delete, and list food menus:** Implemented full CRUD endpoints for menus, validated against the ERD and tested with sample data.
- **Web Services (Apex.Catering) to add/remove a food item from a menu:** Implemented add/remove endpoints for menu items, validated for correct relationships and persistence.
- **Web Services (Apex.Catering) to book, edit, and cancel food for an event (returning FoodBookingId):** Implemented booking lifecycle endpoints with FoodBookingId confirmation and tested booking updates/cancellations.
- **Create a new Event with title, date, and EventType:** Event creation supports required fields with validation and persistence; newly created events appear in the list.
- **Create, list, and edit Guests:** Guest CRUD is implemented with validation and Bootstrap-styled list/detail views.
- **Book a Guest onto an Event:** Guest bookings can be created via the booking form; booking date and relationships are persisted and listed.
- **List Guests for an Event including total count:** Event guest lists display attendees and totals in a clear table view.
- **Register guest attendance for an Event:** Attendance can be recorded and is shown in guest and booking details.
- **Display Guest details including associated Events and attendance:** Guest details pages list related events with attendance status.
- **Edit an Event (except date and type):** Event edits allow name and reservation updates while preserving event date/type integrity.

### SHOULD Functional Requirements (Completed)
- **Cancel a Guest booking for an upcoming Event:** Guest bookings can be removed from upcoming events via the UI.
- **Reserve an available Venue via Apex.Venues and free any previously associated Venue:** Venue reservation is handled through the Venues API with release logic when changing/cancelling.
- **Display Events list with summary information about Guests and Venue:** Events list includes summary fields and related venue details.
- **Create, list, and edit Staff:** Staff CRUD is implemented with validation and list/detail views.
- **Adjust Event staffing (add/remove staff):** Staffing management supports adding/removing assigned staff.
- **Warnings when no First Aider assigned:** The events list/staffing views surface warnings or separate lists when first aid coverage is missing.
- **Display Staff details including upcoming Events:** Staff detail pages show upcoming assignments for visibility.

## Db initialize
Apex.catering/Data/DbTestDataInitializer
initializes test data for a database related to catering services using Entity Framework.
With EF Core, data access is performed using a model. A model is made up of entity classes and a context object that represents a session with the database. The context object allows querying and saving data.
https://learn.microsoft.com/en-us/ef/core/
