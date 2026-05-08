# Implementation Plan: Admin Module Backend & Database Completion

This plan outlines the necessary Backend (BLL/DAL) and Database (DB) developments to fully support the existing Admin UI/UX in the Library Management System.

## User Review Required

> [!IMPORTANT]
> This plan focuses strictly on **Backend and Database** layers for the **Admin** functional group. UI/UX changes are out of scope as per user request.

> [!NOTE]
> Business Rules (e.g., fine calculation logic, maximum borrow limits) are based on standard library management practices and existing `THAMSO` models.

## Proposed Functional Specifications (For BA Review)

### 1. Reader Management (Quản lý Độc giả)
- **View All Readers**: Retrieve a comprehensive list of readers with their current status and fine balance.
- **Reader Lifecycle**: 
    - Support updating reader profiles (Name, Address, Email, Birthdate).
    - Support changing "Reader Type" (LoaiDocGia) to adjust borrowing privileges.
    - Implement a "Soft Delete" or "Deactivate" mechanism to prevent data loss for historical records.

### 2. Circulation Management (Quản lý Mượn/Trả)
- **Book Return Workflow**: 
    - Admin identifies a reader and selects books to return.
    - System calculates **Late Fines** based on `NgayTra` vs `HanTra`.
    - Business Rule: `Fine = (Days Overdue) * (FinePerDay parameter)`.
    - Transactional updates: Create `PHIEUTRA`, update `CHITIETPHIEUMUON` to 'Returned', and set `SACH.TinhTrang` to 'Available'.
- **Debt Management**: Automatically update `DOCGIA.TongNo` when a fine is generated.

### 3. Library Rules Management (Quản lý Quy định)
- **Dynamic Parameters**: Enable Admin to modify system-wide constraints stored in the `THAMSO` table:
    - Min/Max age for readers.
    - Card validity duration (months).
    - Maximum years between book publication and entry.
    - Maximum books a reader can hold.
    - Maximum borrowing days per book.

### 4. Advanced Reporting
- **Top Borrowed Books**: Statistic by category and time range.
- **Late Return Report**: List readers with overdue books and cumulative fines.

---

## Technical Proposed Changes

### 1. Database Layer (DB)
- **[NEW] Store Procedures**:
    - `sp_ProcessReturn`: Handles the complex transaction of returning a book (Create PhieuTra, Update status, calculate fine).
    - `sp_GetReaderBorrowingStats`: Optimized query for the Admin Borrowing UI list.
- **[MODIFY] Seed Data**: Update `THAMSO` with standard default values.

### 2. Data Access Layer (DAL)

#### [MODIFY] [DocGiaRepository.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/DAL/DocGiaRepository.cs)
- Add `GetAllReaders()`: Returns `List<DocGia>`.
- Add `UpdateReader(DocGia reader)`: Updates basic info.
- Add `DeleteReader(int maDocGia)`: Handles logical deletion.
- Add `GetReadersWithBorrowedCount()`: For the Admin Borrowing UI list.

#### [NEW] [ReturnRepository.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/DAL/ReturnRepository.cs)
- Handles interaction with `PHIEUTRA` and `CHITIETPHIEUTRA` tables.

#### [NEW] [SettingsRepository.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/DAL/SettingsRepository.cs)
- Generic methods to Get/Set values in the `THAMSO` table.

### 3. Business Logic Layer (BLL)

#### [NEW] [ReaderManagementService.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/BLL/ReaderManagementService.cs)
- Encapsulates logic for managing reader accounts and profiles.

#### [NEW] [CirculationService.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/BLL/CirculationService.cs)
- **Logic**: `ReturnBook(int maSach, int maDocGia, DateTime returnDate)`.
- Calculates overdue days and fine amounts.

#### [NEW] [LibrarySettingsService.cs](file:///c:/Users/DELL/source/repos/library-management-system/THUVIENZ/BLL/LibrarySettingsService.cs)
- Validates and updates library rules (e.g., ensuring `MinAge < MaxAge`).

---

## Verification Plan

### Automated Tests
- Unit tests for `CirculationService` to verify fine calculation logic with different date ranges.
- DAL integration tests to ensure SQL Transactions (Return Book) rollback correctly on failure.

### Manual Verification
- Execute SQL scripts and verify schema changes in SQL Server Management Studio.
- Verify through the existing Admin UI that data is correctly loaded and actions (like clicking "Return") trigger the new Backend logic.
