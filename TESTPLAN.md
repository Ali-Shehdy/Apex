# Test Plan

## Scope
Manual verification of functional requirements across Apex.Events, Apex.Catering, and Apex.Venues integrations.

## Preconditions
- Solution builds successfully.
- Databases initialized and seeded where appropriate.
- Apex.Events, Apex.Catering, and Apex.Venues services running.
- Test accounts for roles: Manager, TeamLeader, and normal user.
- Development auto-login disabled unless explicitly testing it.

---

## General Requirements

### Starter project copied
1. Confirm solution folders exist: `Apex.Events`, `Apex.Catering`, `Apex.Venues`, and `Apex.Events.web`.

**Expected:** Starter project structure is present.

### README exists with key info
1. Open `README.md`.
2. Verify project name and description are present.

**Expected:** README includes at least project name and description.

### README: problems, challenges, alternatives, justification, concise
1. Read `README.md`.
2. Verify it lists key problems/challenges and alternative solutions.
3. Verify it includes justification with references to knowledge sources.
4. Confirm it is clear and concise.

**Expected:** README meets all documentation requirements.

### Code comments included
1. Spot-check a few core modules (Events, Guests, Staffings) for meaningful comments.

**Expected:** Appropriate comments exist where logic is non-trivial.

### UI/workflow customization beyond scaffolding
1. Navigate the web UI.
2. Confirm layout, navigation, and pages show customization beyond default scaffolding.

**Expected:** UI is customized.

---

## MUST Requirements

### Catering: food items CRUD (Apex.Catering service)
1. Call API to list food items.
2. Create a new food item.
3. Edit the food item.
4. Delete the food item.

**Expected:** All CRUD operations succeed and reflect in list.

### Catering: food menus CRUD (Apex.Catering service)
1. List menus.
2. Create a menu.
3. Edit menu details.
4. Delete the menu.

**Expected:** All CRUD operations succeed and reflect in list.

### Catering: add/remove item from menu
1. Add a food item to a menu via API.
2. Verify item appears in menu details.
3. Remove the item.

**Expected:** Menu updates correctly.

### Catering: book/edit/cancel food for event
1. Book food for an event.
2. Confirm API returns `FoodBookingId`.
3. Edit the food booking.
4. Cancel the booking.

**Expected:** Booking lifecycle works; booking id returned on create.

### Create new event (title, date, type)
1. In Apex.Events, create an event with name, date, and event type.

**Expected:** Event is saved and appears in events list.

### Create/list/edit guests
1. Create a new guest.
2. List guests.
3. Edit guest details.

**Expected:** Guest data is persisted and updates are visible.

### Book guest onto event
1. Select an event and book a guest.

**Expected:** Guest appears on event guest list.

### List guests for event + total count
1. Open event guest list.

**Expected:** List shows guests and total count is correct.

### Register guest attendance
1. Mark guest attendance for an event.

**Expected:** Attendance status saved and displayed.

### Guest details with associated events + attendance
1. Open guest details.

**Expected:** Shows associated events and attendance info.

### Edit event (except date/type)
1. Edit an event’s allowed fields.
2. Confirm date/type are not editable.

**Expected:** Event updates except date/type.

---

## SHOULD Requirements

### Cancel guest booking from upcoming event
1. Remove guest booking for a future event.

**Expected:** Booking removed.

### Reserve available venue for event via Apex.Venues
1. Select an event.
2. Reserve a venue via the service.
3. Update reservation to a different venue.

**Expected:** Venue assigned, previous venue released.

### Event list with guest + venue summary
1. Open events list.

**Expected:** Summary includes guest count and venue.

### Create/list/edit staff
1. Create a staff entry.
2. List staff.
3. Edit staff details.

**Expected:** Staff CRUD works.

### Adjust staffing for an event
1. Add staff to event.
2. Remove staff from event.

**Expected:** Staffing updates are saved.

### Warning if no first aider assigned
1. Ensure an event has no staff with role First Aider.
2. View event list/staffing view.

**Expected:** Warning is shown.

### Staff details with upcoming event assignments
1. Open staff details page.

**Expected:** Upcoming assigned events shown.

### Cancel (soft delete) event
1. Cancel an event.
2. Verify venue and staff freed.

**Expected:** Event marked cancelled; resources released.

---

## WOULD Requirements

### Event details include venue, staff, guests
1. Open event details page.

**Expected:** Shows detailed venue, staff, and guests.

### Permanently remove personal data (anonymise guest)
1. Anonymise a guest.

**Expected:** Personal data removed; record remains.

### Filtered venue list for new event creation
1. Filter venues by event type and date range.
2. Create new event from selected venue.

**Expected:** Event created using selected venue.

### Staffing warning if fewer than 1 staff per 10 guests
1. Create event with guest count > staffing ratio.
2. View event list/staffing view.

**Expected:** Warning is shown.

### User access control restrictions
1. Login as Manager and verify:
   - Can create/edit staff.
   - Can adjust staffing.
   - Can permanently delete (where applicable).
2. Login as TeamLeader and verify:
   - Can adjust staffing.
   - Can permanently delete (where applicable).
   - Cannot create/edit staff.
3. Login as normal user and verify:
   - Cannot create/edit staff.
   - Cannot adjust staffing.
   - Cannot permanently delete.

**Expected:** Access is restricted per requirements.

---

## Notes
- If the environment cannot run services, execute tests locally.
- Use API clients (Swagger/Postman) for Apex.Catering and Apex.Venues endpoints.
- Roles must match exactly: `Manager`, `TeamLeader`.