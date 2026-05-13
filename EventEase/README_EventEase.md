# EventEase - Event Management Application

## Overview
EventEase is a Blazor WebAssembly application for corporate and social event management. Users can browse events, view details, and register for events.

## Project Structure

### Models
- **Event.cs** (`Models/Event.cs`)
  - Represents an event with properties: Id, Name, Date, Location, Description, Capacity, Category

### Services
- **EventService.cs** (`Services/EventService.cs`)
  - Provides mock data for 6 events (Tech Summit, Leadership Workshop, Networking Gala, etc.)
  - Methods:
    - `GetAllEventsAsync()` - Returns all events
    - `GetEventByIdAsync(int id)` - Returns a specific event
    - `GetEventsByCategoryAsync(string category)` - Returns events by category

### Components
- **EventCard.razor** (`Components/EventCard.razor`)
  - Reusable component to display event information
  - Displays: Event name, date, location, description, capacity, category
  - Includes buttons to view details and register
  - Data binding via `[Parameter] Event Event` property

### Pages

#### 1. Home.razor (`Pages/Home.razor`)
- Route: `/`
- Welcome page with hero section
- Features overview
- Call-to-action button to browse events

#### 2. Events.razor (`Pages/Events.razor`)
- Route: `/events`
- Event list page
- Displays all events in a responsive grid layout
- Uses EventCard component for each event
- Shows loading spinner while fetching data

#### 3. EventDetails.razor (`Pages/EventDetails.razor`)
- Route: `/events/{id:int}`
- Detailed view of a single event
- Displays comprehensive event information
- Sidebar with quick event summary
- Buttons to go back to events list or register

#### 4. Registration.razor (`Pages/Registration.razor`)
- Route: `/events/{id:int}/register`
- Event registration form
- Fields: First Name, Last Name, Email, Phone, Company, Special Requirements
- Form validation using EditForm and DataAnnotationsValidator
- Shows confirmation message after registration
- Event summary sidebar

## Routing
The application uses Blazor's built-in routing system:
- `/` - Home page
- `/events` - Event list
- `/events/{id}` - Event details
- `/events/{id}/register` - Registration form

## Styling
Custom CSS added to `wwwroot/css/app.css`:
- Event card styling with hover effects
- Responsive grid layout for event list
- Bootstrap Icons integration
- Purple gradient theme for headers
- Mobile-friendly responsive design

## Navigation
Updated `Layout/NavMenu.razor` to include Events link in the navigation menu.

## Dependency Injection
EventService is registered as a scoped service in `Program.cs` for dependency injection throughout the application.

## Features Implemented
✅ Event browsing with grid layout
✅ Event card component with data binding
✅ Detailed event view
✅ Registration form with validation
✅ Seamless routing between pages
✅ Mock data service
✅ Responsive design
✅ Bootstrap Icons integration
✅ Loading states
✅ Error handling (event not found)

## How to Run
1. Build the solution (already successful)
2. Run the application (F5 or Ctrl+F5)
3. Navigate to the Events page to see the event list
4. Click on any event to view details or register

## Next Steps (Future Enhancements)
- Add actual form validation attributes to registration model
- Implement backend API integration
- Add search and filter functionality
- Implement actual registration persistence
- Add user authentication
- Include image uploads for events
- Add calendar view of events
