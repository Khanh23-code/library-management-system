# Implementation Plan v4: Search Suggestions & MVVM Compliant Image Upload

This revised plan addresses the architectural violations in v3, enforcing strict MVVM patterns, robust image storage, and DRY principles.

## User Review Required

> [!IMPORTANT]
> **Strict MVVM Compliance**: All UI-triggered actions (Search, Image Selection) will be routed through the `BookManagementViewModel` via `RelayCommand`.
> **Image Management**: Images will be persistent in the local file system (`Assets/Images`) with unique GUID naming to prevent collisions.

## Open Questions for Tech Lead

1. **Dialog Service Pattern**: To keep the ViewModel free of UI dependencies (`OpenFileDialog`), do you prefer implementing a simple `IDialogService` or using an Event Messenger/Service locator pattern to trigger the file picker?
2. **Path Storage**: For the `HinhAnh` column in the DB, should we store the full relative path `Assets/Images/[GUID].jpg` or just the filename `[GUID].jpg`?
3. **Suggestion DTO**: Besides the text match, should the suggestion DTO include the `MaSach` or `Loai` (Title vs Author) for better UI categorization?

## Proposed Changes

---

### [Component] Search & Suggestions (BLL)

Following the DRY principle, suggestions logic will be integrated into the existing `SearchService`.

#### [MODIFY] [SearchService.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/BLL/SearchService.cs)
- Add `GetSuggestionsAsync(string query)` returning `IEnumerable<SuggestionDto>`.
- **SuggestionDto**: New DTO with fields: `DisplayText`, `SecondaryText`, `Category` (Title/Author).

---

### [Component] Book Management (ViewModel & Models)

Refactoring to handle image selection logic and persistence.

#### [MODIFY] [BookManagementViewModel.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/ViewModels/BookManagementViewModel.cs)
- Add `SelectImageCommand` (RelayCommand).
- **Logic**: Trigger dialog (via service), copy file to `Assets/Images`, generate GUID filename, and update `NewBook.HinhAnh` with the relative path.

---

### [Component] Add Book UI (View)

Ensuring zero logic in code-behind.

#### [MODIFY] [AddBookPopup.xaml](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/Views/Popups/AddBookPopup.xaml)
- Bind the "Choose Image" button `Command` to `SelectImageCommand` in the ViewModel.
- Bind the `Source` of the cover `Image` control to `NewBook.HinhAnh` with a proper path converter.

#### [MODIFY] [AddBookPopup.xaml.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/Views/Popups/AddBookPopup.xaml.cs)
- **[DELETE]** Remove any click handlers related to file selection or image loading.

---

## Verification Plan

### Automated Tests
- Build verification.
- Unit test for `SearchService.GetSuggestionsAsync` to ensure top-10 limit.

### Manual Verification
1. Open Add Book popup.
2. Click the image border.
3. Select a large image.
4. Verify the file is successfully copied to `THUVIENZ/Assets/Images/` with a GUID name.
5. Verify the preview updates instantly via Data Binding.
