# Implementation Plan: Search Suggestions & Book Cover Upload

This plan outlines the steps to implement a backend service for search suggestions and update the book addition UI to support image cover selection with proper file filters.

## User Review Required

> [!IMPORTANT]
> The search suggestion service will be implemented as a backend-only feature for now, as requested. The UI integration will be handled in a later phase.
> The image upload functionality will use the standard Windows `OpenFileDialog`.

## Proposed Changes

---

### [Component] Search Suggestions Service

We will create a dedicated service to fetch quick suggestions (titles and authors) from the database to power an eventual "Autocomplete" feature.

#### [NEW] [SearchSuggestionService.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/BLL/SearchSuggestionService.cs)
- Create a service that queries the `SACH` table.
- Implement `GetSuggestionsAsync(string query)` returning a list of strings (titles/authors).
- Limit results to top 10 for performance.

---

### [Component] Admin Books - Cover Image Upload

Update the `AddBookPopup` to allow admins to select an image file for the book cover.

#### [MODIFY] [AddBookPopup.xaml](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/Views/Popups/AddBookPopup.xaml)
- Add an `x:Name` to the cover image `Image` control (if exists) or the `Border`.
- Add an `Image` control inside the border to display the selected cover.

#### [MODIFY] [AddBookPopup.xaml.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/Views/Popups/AddBookPopup.xaml.cs)
- Implement a click handler for the "Choose Image" button.
- Use `Microsoft.Win32.OpenFileDialog`.
- Set the filter to: `Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png`.
- Update the UI to show the selected image.

---

## Verification Plan

### Automated Tests
- Build check to ensure no syntax errors.

### Manual Verification
1. Open "Quản lý sách".
2. Click "+ Thêm sách mới".
3. Click into the "Ảnh bìa sách" area.
4. Verify that the Windows File Explorer opens with the filter restricted to image files.
5. Select an image and verify it appears in the popup.
