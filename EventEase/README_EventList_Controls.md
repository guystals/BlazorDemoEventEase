# Event List Controls - Quick Reference

## UI Controls Overview

### 1. Search Bar (Left Section)
```
┌─────────────────────────────────────────┐
│ 🔍 Search events by name or location... │
└─────────────────────────────────────────┘
```
- **Type to search**: Searches name, location, and description
- **Auto-filter**: Results appear after 300ms
- **Clear button (X)**: Removes search and shows all events
- **Case-insensitive**: Finds matches regardless of case

### 2. Category Filter (Dropdown)
```
┌──────────────────┐
│ All Categories  ▼│
├──────────────────┤
│ Technology       │
│ Business         │
│ Social          │
│ Corporate       │
│ Education       │
│ Networking      │
└──────────────────┘
```
- **Select category**: Filters events instantly
- **All Categories**: Shows all events (default)

### 3. Sort Options (Dropdown)
```
┌──────────────────────┐
│ Date (Earliest)     ▼│
├──────────────────────┤
│ Date (Earliest)      │
│ Date (Latest)        │
│ Name (A-Z)          │
│ Name (Z-A)          │
│ Capacity (High-Low) │
└──────────────────────┘
```
- **Select sort**: Re-orders events immediately
- **Default**: Date (Earliest first)

### 4. Items Per Page (Dropdown)
```
┌─────────────┐
│ 25 per page▼│
├─────────────┤
│ 10 per page │
│ 25 per page │
│ 50 per page │
│ 100 per page│
└─────────────┘
```
- **Select size**: Changes pagination immediately
- **Default**: 25 per page
- **Recommended**: 25-50 for best performance

### 5. Results Counter
```
Showing 25 of 500 events
```
- Shows filtered count vs. total count
- Updates in real-time

### 6. Pagination Controls
```
┌──────────────────────────────────────────┐
│ ⏮ ◀ [1] 2 3 4 5 ... ▶ ⏭                │
│        Page 1 of 20                      │
└──────────────────────────────────────────┘
```

**Buttons**:
- `⏮` - Jump to first page
- `◀` - Previous page
- `[1] 2 3 4 5` - Direct page selection (current page highlighted)
- `▶` - Next page
- `⏭` - Jump to last page

**Behavior**:
- Disabled buttons are grayed out
- Shows 5 page numbers at a time
- Adjusts page numbers based on current position

## Keyboard Shortcuts

### Tab Navigation:
1. Search box
2. Clear search button (if visible)
3. Category dropdown
4. Sort dropdown
5. Items per page dropdown
6. Event cards
7. Pagination buttons

### Enter Key:
- In search box: Applies filter immediately
- On buttons: Activates the button

## Search Examples

### By Name:
```
"Tech Summit" → Finds "Annual Tech Summit 2025"
"leadership" → Finds "Corporate Leadership Workshop"
```

### By Location:
```
"San Francisco" → All events in San Francisco
"CA" → All events in California
```

### By Description:
```
"networking" → Events with networking in description
"AI" → Events about artificial intelligence
```

## Filter Combinations

### Example 1: Tech Events in 2025
- **Category**: Technology
- **Sort**: Date (Earliest)
- **Result**: All tech events chronologically

### Example 2: Recent Corporate Events
- **Search**: "corporate"
- **Category**: Corporate
- **Sort**: Date (Latest)
- **Result**: Latest corporate events first

### Example 3: Large Capacity Events
- **Sort**: Capacity (High-Low)
- **Items per page**: 10
- **Result**: Top 10 largest events

## Performance Tips

### For Best Performance:
1. ✅ Use pagination (25-50 items per page)
2. ✅ Apply filters to reduce dataset
3. ✅ Use search for specific events
4. ✅ Sort after filtering for faster results

### Avoid:
1. ❌ 100+ items per page with large datasets
2. ❌ Rapid typing in search (debouncing handles this)
3. ❌ Frequent page changes (affects UX)

## Mobile Experience

### Responsive Layout:
- **Mobile**: Controls stack vertically
- **Tablet**: 2-3 controls per row
- **Desktop**: All controls in one row

### Touch Optimization:
- Large tap targets (44x44px minimum)
- Swipe support on pagination (future)
- Dropdown friendly for touch

## Empty States

### No Events:
```
┌────────────────────────────────────┐
│ ℹ No events available             │
│   [Create Your First Event]       │
└────────────────────────────────────┘
```

### No Search Results:
```
┌────────────────────────────────────┐
│ ℹ No events found matching your   │
│   search criteria.                │
│   [Clear Filters]                 │
└────────────────────────────────────┘
```

## Loading State
```
┌────────────────────────────────────┐
│            ⏳                      │
│      Loading events...            │
└────────────────────────────────────┘
```
- Shows while fetching data
- Prevents interaction during load

## Advanced Usage

### Multi-Criteria Search:
1. Enter search term (e.g., "summit")
2. Select category (e.g., "Technology")
3. Choose sort (e.g., "Date (Earliest)")
4. Result: Tech summits in chronological order

### Quick Reset:
1. Click "Clear Search" (X button)
2. Select "All Categories"
3. Result: Back to default view

## Accessibility

### Screen Reader Support:
- All controls are labeled
- Page changes announced
- Result counts read aloud
- Loading states communicated

### Keyboard Only:
- Full navigation via Tab
- Enter/Space to activate
- Arrow keys in dropdowns
- Escape to close dropdowns

## Browser Compatibility

✅ Chrome/Edge (latest)
✅ Firefox (latest)
✅ Safari (latest)
✅ Mobile browsers (iOS/Android)

## Troubleshooting

### Search Not Working:
- Wait 300ms after typing
- Check for typos
- Try broader search terms

### No Results:
- Clear all filters
- Check spelling
- Verify events exist

### Pagination Issues:
- Refresh page
- Clear browser cache
- Check console for errors

## Integration with API

### Ready for Backend:
Current implementation can easily switch to API calls:

```csharp
// Current: In-memory
var events = await EventService.GetAllEventsAsync();

// Future: API endpoint
var events = await Http.GetFromJsonAsync<List<Event>>(
    $"api/events?page={page}&category={category}&search={search}");
```

## Summary

The event list provides:
- ⚡ Fast search with debouncing
- 🎯 Precise filtering by category
- 📊 Flexible sorting options
- 📄 Efficient pagination
- 📱 Mobile-friendly interface
- ♿ Fully accessible
- 🚀 Scalable to large datasets

Perfect for managing hundreds or thousands of events! 🎉
