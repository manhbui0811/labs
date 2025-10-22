# 🚀 HƯỚNG DẪN LUỒNG GỌI API CHO CÁC MÀN HÌNH UI

## 📋 MỤC LỤC
1. [Entities List](#1-entities-list)
2. [Entity Detail (Overview)](#2-entity-detail-overview)
3. [Branches & POS List](#3-branches--pos-list)
4. [Branch Detail](#4-branch-detail)
5. [Merchant Profiles List](#5-merchant-profiles-list)
6. [Merchant Profile Detail](#6-merchant-profile-detail)
7. [Terminals List](#7-terminals-list)
8. [Devices List](#8-devices-list)
9. [Common Patterns](#9-common-patterns)

---

## 1. ENTITIES Select

Sau khi chọn Entity xong url sẽ có dạng /{entityId}/Các chức năng sau...

## 3. BRANCHES

### 🎯 Màn hình: Quản lý chi nhánh url: /{entityId}/branch

#### 3.1. Danh sách chi nhánh
**DTO**: List<EntityBranchListDto>
EntityBranchListDto {
    id?: string;
    entityId?: string;
    branchCode?: string;
    branchName?: string;
    province?: string | undefined;
    ward?: string | undefined;
    contactPhone?: string | undefined;
    status?: ActiveStatus;
    createdAt?: Date;
    latitude?: number | undefined;
    longitude?: number | undefined;
}
**Funcition**: client.GetBranchesByEntity(entityId);
**Yêu cầu**
- Không có chức năng filter;
- Button chức năng(icon): Create, Refresh;
- Load tất cả các branches của entity;
- Hiển thị các trường:
  branchCode,branchName,province,ward,contactPhone,status,createdAt
- Các chức năng của row:
  + Chỉnh sửa chi nhánh
  + Danh sách điểm bán của chi nhánh: click vào đi sang url /{entityId}/point-off-sale?branchId={branchId}


#### 3.2. Chỉnh sửa chi nhánh
**DTO**: UpdateEntityBranchRequest 
UpdateEntityBranchRequest {
    branchName?: string;
    address?: string | undefined;
    provinceCode?: string | undefined;
    wardCode?: string | undefined;
    postalCode?: string | undefined;
    latitude?: number | undefined;
    longitude?: number | undefined;
    contactPhone?: string | undefined;
    contactEmail?: string | undefined;
    managerName?: string | undefined;
}
**Funcition**: client.updateEntityBranch(id: string, updateEntityBranchRequest: UpdateEntityBranchRequest): Observable<void>;
**Yêu cầu**
- Chỉnh sửa chi nhánh hiển thị trên modal;
- Cập nhật thành công: thông báo thành công, update lại vào bảng hiển thị;

#### 3.3. Tạo mới chi nhánh
**DTO**: CreateEntityBranchRequest 
CreateEntityBranchRequest {
    branchCode?: string;
    branchName?: string;
    parentBranchId?: string | undefined;
    address?: string | undefined;
    provinceCode?: string | undefined;
    wardCode?: string | undefined;
    postalCode?: string | undefined;
    latitude?: number | undefined;
    longitude?: number | undefined;
    contactPhone?: string | undefined;
    contactEmail?: string | undefined;
    managerName?: string | undefined;
}
**Funcition**: client.createEntityBranch(entityId: string, createEntityBranchRequest: CreateEntityBranchRequest): Observable<string>;
**Yêu cầu**
- Tạo mới chi nhánh hiển thị trên modal;
- Tạo mới thành công: thông báo thành công, update lại vào bảng hiển thị;



## 4. POINT_OF_SALES

### 🎯 Màn hình: Quản lý điểm bán url: /{entityId}/point-of-sale?{search-param}

#### 3.1. Danh sách điểm bán
**DTO**: PagedResultOfPointOfSaleListDto
PagedResultOfPointOfSaleListDto {
    items?: PointOfSaleListDto[];
    totalCount?: number;
    pageNumber?: number;
    pageSize?: number;
    totalPages?: number;
    hasPreviousPage?: boolean;
    hasNextPage?: boolean;
}
PointOfSaleListDto{
    id?: string;
    entityId?: string;
    posCode?: string;
    posName?: string;
    city?: string | undefined;
    status?: ActiveStatus;
    terminalCount?: number;
    createdAt?: Date;
}

**Funcition**: client.getPointOfSales(entityId: string | null | undefined,branchId: string | null, status: ActiveStatus | null | undefined, searchTerm: string | null | undefined, page: number, pageSize: number): Observable<PagedResultOfPointOfSaleListDto>;
**Yêu cầu**
- Filter các trường sau:
  + branch: Dropdown (sử dụng api get)
- Button chức năng(icon): Create, Refresh;
- Load tất cả các branches của entity;
- Hiển thị các trường:
  branchCode,branchName,province,ward,contactPhone,status,createdAt
- Các chức năng của row:
  + Chỉnh sửa chi nhánh
  + Danh sách điểm bán của chi nhánh: click vào đi sang url /{entityId}/point-off-sale?branchId={branchId}


#### 3.2. Chỉnh sửa chi nhánh
**DTO**: UpdateEntityBranchRequest 
UpdateEntityBranchRequest {
    branchName?: string;
    address?: string | undefined;
    provinceCode?: string | undefined;
    wardCode?: string | undefined;
    postalCode?: string | undefined;
    latitude?: number | undefined;
    longitude?: number | undefined;
    contactPhone?: string | undefined;
    contactEmail?: string | undefined;
    managerName?: string | undefined;
}
**Funcition**: client.updateEntityBranch(id: string, updateEntityBranchRequest: UpdateEntityBranchRequest): Observable<void>;
**Yêu cầu**
- Chỉnh sửa chi nhánh hiển thị trên modal;
- Cập nhật thành công: thông báo thành công, update lại vào bảng hiển thị;

#### 3.3. Tạo mới chi nhánh
**DTO**: CreateEntityBranchRequest 
CreateEntityBranchRequest {
    branchCode?: string;
    branchName?: string;
    parentBranchId?: string | undefined;
    address?: string | undefined;
    provinceCode?: string | undefined;
    wardCode?: string | undefined;
    postalCode?: string | undefined;
    latitude?: number | undefined;
    longitude?: number | undefined;
    contactPhone?: string | undefined;
    contactEmail?: string | undefined;
    managerName?: string | undefined;
}
**Funcition**: client.createEntityBranch(entityId: string, createEntityBranchRequest: CreateEntityBranchRequest): Observable<string>;
**Yêu cầu**
- Tạo mới chi nhánh hiển thị trên modal;
- Tạo mới thành công: thông báo thành công, update lại vào bảng hiển thị;




## 4. POINT_OF_SALE

### 🎯 Màn hình: Branch Detail Page
**File:** `page-branch-detail` section

### API Calls

#### 4.1. Load Branch Overview
```javascript
async function loadBranchDetail(branchId) {
  try {
    // 1. Get branch details
    const branchResponse = await fetch(`/api/branches/${branchId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const branch = await branchResponse.json();
    
    // 2. Get POS list for this branch
    const posResponse = await fetch(
      `/api/pos?branchId=${branchId}&pageNumber=1&pageSize=100`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const posData = await posResponse.json();
    
    // Render branch info
    renderBranchInfo(branch);
    
    // Render POS table
    renderBranchPOSList(posData.items);
    
  } catch (error) {
    console.error('Error loading branch detail:', error);
  }
}
```

#### 4.2. Alternative: Using Existing Queries
```javascript
async function loadBranchDetailAlt(branchId) {
  try {
    // Use existing GetEntityBranchByIdQuery
    const response = await fetch(`/api/entity-branches/${branchId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const branch = await response.json();
    
    // Load POS separately
    const posResponse = await fetch(
      `/api/pos?entityId=${branch.entityId}&pageNumber=1&pageSize=100`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const posData = await posResponse.json();
    
    // Filter POS for this branch if needed
    // (Note: POS doesn't have branchId in domain, so filter client-side or by location)
    
    renderBranchInfo(branch);
    renderBranchPOSList(posData.items);
    
  } catch (error) {
    console.error('Error loading branch:', error);
  }
}
```

---

## 5. MERCHANT PROFILES LIST

### 🎯 Màn hình: Merchant Profiles List
**File:** `page-mps` section

### API Calls

#### 5.1. Load Merchant Profiles List
```javascript
async function loadMerchantProfiles(entityId, pageNumber = 1) {
  try {
    const response = await fetch(
      `/api/merchant-profiles?entityId=${entityId}&pageNumber=${pageNumber}&pageSize=50`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const data = await response.json();
    
    /*
    Response: PagedResult<MerchantProfileDto>
    {
      items: [{
        id: "...",
        merchantCode: "MID-001",
        merchantName: "Cafe Store",
        entityId: "...",
        status: "Active",
        merchantCategoryId: "...",
        riskRating: 3,
        ...
      }],
      totalCount: 100,
      pageNumber: 1,
      pageSize: 50
    }
    */
    
    renderMerchantProfilesTable(data.items, data.totalCount, pageNumber);
    
  } catch (error) {
    console.error('Error loading merchant profiles:', error);
  }
}
```

#### 5.2. Search Merchant Profiles (Typeahead)
```javascript
const searchMerchantProfiles = debounce(async (searchTerm, entityId) => {
  if (searchTerm.length < 2) return;
  
  try {
    // ✅ NEW API: Lookup endpoint
    const response = await fetch(
      `/api/merchant-profiles/lookup?entityId=${entityId}&term=${encodeURIComponent(searchTerm)}&limit=20`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const results = await response.json();
    
    /*
    Response: List<MerchantProfileLookupDto>
    [{
      id: "...",
      merchantCode: "MID-001",
      merchantName: "...",
      entityId: "...",
      entityName: "...",
      status: "Active"
    }]
    */
    
    showAutocompleteResults(results);
    
  } catch (error) {
    console.error('Error searching merchants:', error);
  }
}, 300);
```

#### 5.3. Create Merchant Profile
```javascript
async function createMerchantProfile(formData) {
  try {
    const response = await fetch('/api/merchant-profiles', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        entityId: formData.entityId,
        merchantCode: formData.merchantCode,
        merchantName: formData.merchantName,
        merchantCategoryId: formData.merchantCategoryId,
        riskRating: formData.riskRating,
        status: 'Pending',
        // ... other fields
      })
    });
    
    if (!response.ok) throw new Error('Failed to create merchant profile');
    
    const merchantId = await response.json();
    
    // Refresh list
    await loadMerchantProfiles(formData.entityId);
    
    return merchantId;
    
  } catch (error) {
    console.error('Error creating merchant:', error);
    throw error;
  }
}
```

---

## 6. MERCHANT PROFILE DETAIL

### 🎯 Màn hình: Merchant Profile Detail Page
**File:** `page-mp-detail` section

### API Calls

#### 6.1. Load Merchant Profile Overview
```javascript
async function loadMerchantProfileDetail(merchantId) {
  try {
    // 1. Get merchant profile
    const merchantResponse = await fetch(`/api/merchant-profiles/${merchantId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const merchant = await merchantResponse.json();
    
    // 2. Get terminals for this merchant
    const terminalsResponse = await fetch(
      `/api/terminals?merchantProfileId=${merchantId}&pageNumber=1&pageSize=100`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const terminalsData = await terminalsResponse.json();
    
    // Render merchant info
    renderMerchantInfo(merchant);
    
    // Render terminals table
    renderMerchantTerminals(terminalsData.items);
    
  } catch (error) {
    console.error('Error loading merchant detail:', error);
  }
}
```

---

## 7. TERMINALS LIST

### 🎯 Màn hình: Terminals List
**File:** `page-terminals` section

### API Calls

#### 7.1. Load Terminals List with Counters
```javascript
async function loadTerminalsPage(entityId) {
  try {
    // 1. Get counters (badges) - Cached 5 minutes
    const countersResponse = await fetch(`/api/terminals/counters?entityId=${entityId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const counters = await countersResponse.json();
    
    /*
    Response: TerminalCountersDto
    {
      totalTerminals: 180,
      byStatus: { "Active": 150, "Inactive": 30 },
      byChannel: { "CARD": 100, "QR": 50, "ECOM": 30 },
      boundDevicesCount: 120
    }
    */
    
    // Render badges/filters
    renderTerminalCounters(counters);
    
    // 2. Load terminals list
    const terminalsResponse = await fetch(
      `/api/terminals?entityId=${entityId}&pageNumber=1&pageSize=50`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const data = await terminalsResponse.json();
    
    renderTerminalsTable(data.items, data.totalCount);
    
  } catch (error) {
    console.error('Error loading terminals:', error);
  }
}
```

#### 7.2. Search Terminals (Typeahead)
```javascript
const searchTerminals = debounce(async (searchTerm) => {
  if (searchTerm.length < 2) return;
  
  try {
    // ✅ NEW API: Lookup endpoint
    const response = await fetch(
      `/api/terminals/lookup?term=${encodeURIComponent(searchTerm)}&limit=20`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const results = await response.json();
    
    /*
    Response: List<TerminalLookupDto>
    [{
      id: "...",
      tidCode: "TID001",
      midCode: "MID-001",
      terminalCode: "QR-HCM-01",
      channelType: "CARD",
      status: "Active"
    }]
    */
    
    showAutocompleteResults(results);
    
  } catch (error) {
    console.error('Error searching terminals:', error);
  }
}, 300);
```

#### 7.3. Get Terminal Current Bindings
```javascript
async function loadTerminalBindings(terminalId) {
  try {
    // ✅ NEW API: Current bindings
    const response = await fetch(`/api/terminals/${terminalId}/bindings/current`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const bindings = await response.json();
    
    /*
    Response: List<DeviceBindingDto>
    [{
      bindingId: "...",
      deviceId: "...",
      deviceCode: "DEV-001",
      deviceSerialNumber: "SN-1001",
      terminalId: "...",
      bindingFrom: "2025-01-15T08:00:00Z",
      status: "Active"
    }]
    */
    
    renderTerminalBindings(bindings);
    
  } catch (error) {
    console.error('Error loading bindings:', error);
  }
}
```

#### 7.4. Create Terminal
```javascript
async function createTerminal(formData) {
  try {
    const response = await fetch('/api/terminals', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        merchantProfileId: formData.merchantProfileId,
        pointOfSaleId: formData.pointOfSaleId || null,
        settlementAccountId: formData.settlementAccountId || null,
        terminalCode: formData.terminalCode,
        tidCode: formData.tidCode, // For CARD terminals
        midCode: formData.midCode,
        channelType: formData.channelType, // "CARD" | "QR" | "ECOM"
        capabilitiesJson: formData.capabilities ? JSON.stringify(formData.capabilities) : null
      })
    });
    
    if (!response.ok) throw new Error('Failed to create terminal');
    
    const terminalId = await response.json();
    
    // Refresh list
    await loadTerminalsPage(formData.entityId);
    
    return terminalId;
    
  } catch (error) {
    console.error('Error creating terminal:', error);
    throw error;
  }
}
```

---

## 8. DEVICES LIST

### 🎯 Màn hình: Devices List
**File:** `page-devs` section

### API Calls

#### 8.1. Load Devices with Counters & Cursor Pagination
```javascript
async function loadDevicesPage(entityId) {
  try {
    // 1. Get counters (badges) - Cached 5 minutes
    const countersResponse = await fetch(`/api/devices/counters?entityId=${entityId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const counters = await countersResponse.json();
    
    /*
    Response: DeviceCountersDto
    {
      totalDevices: 1250,
      byStatus: { "Active": 1100, "Inactive": 100, "Maintenance": 50 },
      byOnlineStatus: { "Online": 980, "Offline": 120, "Idle": 150 },
      byKind: { "POS": 800, "MPOS": 300, "SoftPOS": 150 },
      assignedCount: 1150,
      unassignedCount: 100
    }
    */
    
    // Render badges/filters
    renderDeviceCounters(counters);
    
    // 2. Load devices with cursor pagination (for large datasets)
    await loadDevicesWithCursor(entityId, null);
    
  } catch (error) {
    console.error('Error loading devices page:', error);
  }
}
```

#### 8.2. Cursor-Based Pagination (Recommended for 10k+ devices)
```javascript
let currentCursor = null;

async function loadDevicesWithCursor(entityId, cursor) {
  try {
    // ✅ NEW API: Cursor pagination
    const params = new URLSearchParams({
      entityId: entityId,
      pageSize: 50,
      sortBy: 'CreatedAt',
      sortDescending: true
    });
    
    if (cursor) params.append('cursor', cursor);
    
    const response = await fetch(`/api/devices/cursor?${params}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    
    /*
    Response: CursorPaginatedResult<DeviceListCursorDto>
    {
      items: [{
        id: "...",
        deviceCode: "DEV-001",
        serialNumber: "SN-1001",
        deviceKind: "POS",
        status: "Active",
        onlineStatus: "Online",
        nickname: "Counter 1",
        entityId: "...",
        entityName: "Branch A",
        currentPointOfSaleId: "...",
        currentPointOfSaleName: "Main Counter",
        lastHeartbeatAt: "2025-10-16T10:30:00Z",
        createdAt: "2025-01-15T08:00:00Z"
      }],
      nextCursor: "2025-01-15T08:00:00Z|uuid-here",
      hasMore: true,
      itemCount: 50
    }
    */
    
    // Append to table (for infinite scroll)
    appendDevicesToTable(data.items);
    
    // Store cursor for next page
    currentCursor = data.nextCursor;
    
    // Show/hide "Load More" button
    toggleLoadMoreButton(data.hasMore);
    
  } catch (error) {
    console.error('Error loading devices:', error);
  }
}

// Load next page
async function loadMoreDevices(entityId) {
  if (currentCursor) {
    await loadDevicesWithCursor(entityId, currentCursor);
  }
}
```

#### 8.3. Alternative: Traditional Pagination (for smaller datasets)
```javascript
async function loadDevicesTraditional(entityId, pageNumber = 1) {
  try {
    const response = await fetch(
      `/api/devices?entityId=${entityId}&pageNumber=${pageNumber}&pageSize=50`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const data = await response.json();
    
    renderDevicesTable(data.items, data.totalCount, pageNumber);
    
  } catch (error) {
    console.error('Error loading devices:', error);
  }
}
```

#### 8.4. Search Devices (Typeahead)
```javascript
const searchDevices = debounce(async (searchTerm, entityId) => {
  if (searchTerm.length < 2) return;
  
  try {
    // ✅ NEW API: Lookup endpoint
    const response = await fetch(
      `/api/devices/lookup?entityId=${entityId}&term=${encodeURIComponent(searchTerm)}&limit=20`,
      { headers: { 'Authorization': `Bearer ${token}` } }
    );
    const results = await response.json();
    
    /*
    Response: List<DeviceLookupDto>
    [{
      id: "...",
      deviceCode: "DEV-001",
      serialNumber: "SN-1001",
      deviceKind: "POS",
      status: "Active",
      nickname: "Counter 1"
    }]
    */
    
    showAutocompleteResults(results);
    
  } catch (error) {
    console.error('Error searching devices:', error);
  }
}, 300);
```

#### 8.5. Filter Devices
```javascript
async function filterDevices(entityId, filters) {
  try {
    const params = new URLSearchParams({
      entityId: entityId,
      pageSize: 50,
      sortBy: 'CreatedAt',
      sortDescending: true
    });
    
    if (filters.status) params.append('status', filters.status);
    if (filters.onlineStatus) params.append('onlineStatus', filters.onlineStatus);
    if (filters.deviceKind) params.append('deviceKind', filters.deviceKind);
    if (filters.searchTerm) params.append('searchTerm', filters.searchTerm);
    
    // Use cursor API with filters
    const response = await fetch(`/api/devices/cursor?${params}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    
    renderDevicesTable(data.items);
    currentCursor = data.nextCursor;
    
  } catch (error) {
    console.error('Error filtering devices:', error);
  }
}
```

#### 8.6. Get Device Current Assignment
```javascript
async function loadDeviceAssignment(deviceId) {
  try {
    // ✅ NEW API: Current assignment
    const response = await fetch(`/api/devices/${deviceId}/assignment/current`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    
    if (response.status === 404) {
      // Device not assigned
      renderNoAssignment();
      return;
    }
    
    const assignment = await response.json();
    
    /*
    Response: CurrentAssignmentDto
    {
      assignmentId: "...",
      deviceId: "...",
      deviceCode: "DEV-001",
      pointOfSaleId: "...",
      posCode: "POS-001",
      posName: "Main Counter",
      assignedFrom: "2025-01-15T08:00:00Z",
      status: "Active",
      bindings: [{
        bindingId: "...",
        terminalId: "...",
        tidCode: "TID001",
        midCode: "MID-001",
        channelType: "CARD",
        bindingFrom: "2025-01-15T08:30:00Z"
      }]
    }
    */
    
    renderDeviceAssignment(assignment);
    
  } catch (error) {
    console.error('Error loading assignment:', error);
  }
}
```

#### 8.7. Assign Device to POS
```javascript
async function assignDevice(deviceId, posId, notes) {
  try {
    const response = await fetch('/api/assignments', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        deviceId: deviceId,
        pointOfSaleId: posId,
        notes: notes,
        installLatitude: null,
        installLongitude: null
      })
    });
    
    if (!response.ok) throw new Error('Failed to assign device');
    
    const assignmentId = await response.json();
    
    // Refresh device assignment display
    await loadDeviceAssignment(deviceId);
    
    return assignmentId;
    
  } catch (error) {
    console.error('Error assigning device:', error);
    throw error;
  }
}
```

#### 8.8. Bulk Assign Devices
```javascript
async function bulkAssignDevices(assignments) {
  try {
    // ✅ NEW API: Bulk assignment
    const response = await fetch('/api/assignments/bulk', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        items: assignments.map(a => ({
          deviceId: a.deviceId,
          pointOfSaleId: a.posId,
          notes: a.notes
        }))
      })
    });
    
    if (!response.ok) throw new Error('Bulk assignment failed');
    
    const result = await response.json();
    
    /*
    Response: BulkAssignResult
    {
      totalItems: 100,
      successCount: 98,
      failureCount: 2,
      errors: [{
        deviceId: "...",
        error: "Device not found"
      }]
    }
    */
    
    // Show results
    showBulkAssignmentResults(result);
    
    return result;
    
  } catch (error) {
    console.error('Error bulk assigning:', error);
    throw error;
  }
}
```

---

## 9. COMMON PATTERNS

### 9.1. Error Handling Pattern
```javascript
async function apiCall(url, options = {}) {
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        'Authorization': `Bearer ${getToken()}`,
        'Content-Type': 'application/json',
        ...options.headers
      }
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || `HTTP ${response.status}`);
    }
    
    return await response.json();
    
  } catch (error) {
    console.error(`API Error [${url}]:`, error);
    
    // Show user-friendly message
    showErrorToast(error.message || 'An error occurred');
    
    throw error;
  }
}
```

### 9.2. Debounce Helper
```javascript
function debounce(func, wait) {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}
```

### 9.3. Loading State Pattern
```javascript
async function loadWithSpinner(loadFunc) {
  const spinner = showSpinner();
  try {
    await loadFunc();
  } finally {
    hideSpinner(spinner);
  }
}

// Usage
await loadWithSpinner(() => loadDevicesPage(entityId));
```

### 9.4. Caching Pattern (Client-side)
```javascript
const cache = new Map();

async function getCachedData(key, fetchFunc, ttl = 5 * 60 * 1000) {
  const cached = cache.get(key);
  
  if (cached && Date.now() - cached.timestamp < ttl) {
    return cached.data;
  }
  
  const data = await fetchFunc();
  cache.set(key, { data, timestamp: Date.now() });
  
  return data;
}

// Usage
const counters = await getCachedData(
  `device-counters-${entityId}`,
  () => fetch(`/api/devices/counters?entityId=${entityId}`).then(r => r.json()),
  5 * 60 * 1000 // 5 minutes
);
```

### 9.5. Infinite Scroll Pattern
```javascript
let isLoading = false;

window.addEventListener('scroll', async () => {
  if (isLoading) return;
  
  const { scrollTop, scrollHeight, clientHeight } = document.documentElement;
  
  if (scrollTop + clientHeight >= scrollHeight - 100) {
    // Near bottom, load more
    if (currentCursor && hasMore) {
      isLoading = true;
      try {
        await loadMoreDevices(selectedEntityId);
      } finally {
        isLoading = false;
      }
    }
  }
});
```

---

## 📊 PERFORMANCE OPTIMIZATION CHECKLIST

### API Level
- ✅ Use composite endpoints (dashboard, branches-pos)
- ✅ Use counters APIs with cache
- ✅ Use cursor pagination for large lists
- ✅ Batch operations (bulk assign)
- ✅ Use lookup endpoints for typeahead

### Client Level
- ✅ Debounce search inputs (300ms)
- ✅ Cache frequently accessed data
- ✅ Lazy load tree nodes
- ✅ Virtual scrolling for 1000+ rows
- ✅ Pagination or infinite scroll
- ✅ Optimize re-renders
- ✅ Use skeleton loaders

### Network Level
- ✅ Parallel requests when possible
- ✅ Request compression
- ✅ ETag/If-None-Match
- ✅ HTTP/2 multiplexing

---

## 🔐 SECURITY NOTES

1. **Always include Authorization header**
   ```javascript
   headers: { 'Authorization': `Bearer ${token}` }
   ```

2. **Validate entity access** - Backend handles RLS automatically

3. **Don't expose sensitive data in URLs** - Use POST body

4. **CSRF protection** - Handled by bearer token

5. **Rate limiting** - Respect 429 responses

---

## 🎯 QUICK REFERENCE MATRIX

| Screen | Load API | Search API | Create API | Counters |
|--------|----------|------------|------------|----------|
| Entities List | `/api/entities/tree/nodes` | `/api/entities/lookup` | `POST /api/entities` | - |
| Entity Detail | `/api/entities/{id}/dashboard` | - | - | ✅ (in dashboard) |
| Branches & POS | `/api/entities/{id}/branches-pos` | - | `POST /api/branches` | - |
| Merchant Profiles | `/api/merchant-profiles` | `/api/merchant-profiles/lookup` | `POST /api/merchant-profiles` | - |
| Terminals | `/api/terminals` | `/api/terminals/lookup` | `POST /api/terminals` | `/api/terminals/counters` |
| Devices | `/api/devices/cursor` | `/api/devices/lookup` | `POST /api/devices` | `/api/devices/counters` |

---

## ✅ IMPLEMENTATION PRIORITY

**Week 1 - Critical (P0):**
- [ ] Entity dashboard composite API
- [ ] Device counters & lookup
- [ ] Terminal counters & lookup
- [ ] Branches & POS combined list

**Week 2 - High (P1):**
- [ ] Cursor pagination for devices
- [ ] Current assignment/bindings
- [ ] All lookup endpoints
- [ ] Search with debouncing

**Week 3 - Medium (P2):**
- [ ] Bulk operations
- [ ] Client-side caching
- [ ] Infinite scroll
- [ ] Virtual scrolling

**Week 4 - Polish:**
- [ ] Error handling
- [ ] Loading states
- [ ] Optimistic updates
- [ ] Performance monitoring