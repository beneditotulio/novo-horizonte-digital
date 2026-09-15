# API Documentation - Novo Horizonte Digital

## Base URL
- **Development**: `https://localhost:5001/api`
- **Production**: `https://api.novo-horizonte.com/api`

## Authentication
Todos os endpoints (exceto login/register) requerem header JWT:
```
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

---

## 1️⃣ Auth Endpoints

### Login
```
POST /auth/login
Content-Type: application/json

Request:
{
  "email": "admin@novo-horizonte.com",
  "password": "Admin@123"
}

Response (200):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "fullName": "Administrador Sistema",
    "email": "admin@novo-horizonte.com",
    "phoneNumber": "+258 21 306 000",
    "role": "Admin"
  }
}

Response (401):
{
  "message": "Invalid credentials"
}
```

### Register
```
POST /auth/register
Content-Type: application/json

Request:
{
  "email": "novo@cliente.com",
  "password": "Senha@123",
  "fullName": "João Silva",
  "phoneNumber": "+258 84 123 4567",
  "role": "Client"
}

Response (200):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 10,
    "fullName": "João Silva",
    "email": "novo@cliente.com",
    "phoneNumber": "+258 84 123 4567",
    "role": "Client"
  }
}

Response (400):
{
  "message": "Email already registered"
}
```

---

## 2️⃣ Area Endpoints

### Get All Areas
```
GET /area
Authorization: Bearer <TOKEN>

Response (200):
[
  {
    "id": 1,
    "name": "Terreno Jovem",
    "dimensions": "15x30",
    "adhesionValue": 50000,
    "monthlyInstallment": 2500,
    "paymentPeriodMonths": 24,
    "housingStandard": "Básico"
  },
  {
    "id": 2,
    "name": "Área-2 Executivo",
    "dimensions": "20x30",
    "adhesionValue": 100000,
    "monthlyInstallment": 5000,
    "paymentPeriodMonths": 24,
    "housingStandard": "Médio"
  }
]
```

### Get Area by ID
```
GET /area/{id}
Authorization: Bearer <TOKEN>

Response (200):
{
  "id": 1,
  "name": "Terreno Jovem",
  "dimensions": "15x30",
  "adhesionValue": 50000,
  "monthlyInstallment": 2500,
  "paymentPeriodMonths": 24,
  "housingStandard": "Básico"
}
```

### Create Area (Admin Only)
```
POST /area
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
{
  "name": "Área Premium",
  "dimensions": "25x40",
  "adhesionValue": 200000,
  "monthlyInstallment": 8000,
  "paymentPeriodMonths": 24,
  "housingStandard": "Alto"
}

Response (201):
{
  "id": 3,
  "name": "Área Premium",
  "dimensions": "25x40",
  "adhesionValue": 200000,
  "monthlyInstallment": 8000,
  "paymentPeriodMonths": 24,
  "housingStandard": "Alto"
}
```

---

## 3️⃣ Lot Endpoints

### Get Lots by Area
```
GET /lot/area/{areaId}
Authorization: Bearer <TOKEN>

Response (200):
[
  {
    "id": 1,
    "name": "Lote 01",
    "areaId": 1
  },
  {
    "id": 2,
    "name": "Lote 02",
    "areaId": 1
  }
]
```

### Create Lot (Admin Only)
```
POST /lot
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
{
  "name": "Lote 03",
  "areaId": 1
}

Response (201):
{
  "id": 3,
  "name": "Lote 03",
  "areaId": 1
}
```

---

## 4️⃣ Plot Endpoints

### Get Available Plots by Lot
```
GET /plot/available/lot/{lotId}
Authorization: Bearer <TOKEN>

Response (200):
[
  {
    "id": 1,
    "plotNumber": "100",
    "areaId": 1,
    "lotId": 1,
    "status": "Available"
  },
  {
    "id": 2,
    "plotNumber": "101",
    "areaId": 1,
    "lotId": 1,
    "status": "Available"
  }
]
```

### Get All Plots by Area and Lot
```
GET /plot/area/{areaId}/lot/{lotId}
Authorization: Bearer <TOKEN>

Response (200):
[
  {
    "id": 1,
    "plotNumber": "100",
    "areaId": 1,
    "lotId": 1,
    "status": "Available"
  },
  {
    "id": 2,
    "plotNumber": "101",
    "areaId": 1,
    "lotId": 1,
    "status": "Reserved"
  }
]
```

### Create Single Plot (Admin Only)
```
POST /plot
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
{
  "areaId": 1,
  "lotId": 1,
  "plotNumber": "150"
}

Response (201):
{
  "id": 50,
  "plotNumber": "150",
  "areaId": 1,
  "lotId": 1,
  "status": "Available"
}
```

### Create Plots in Range (Admin Only)
```
POST /plot/range
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
{
  "areaId": 1,
  "lotId": 1,
  "startNumber": 200,
  "endNumber": 250
}

Response (200):
{
  "message": "51 plots created successfully",
  "plots": [
    {
      "id": 51,
      "plotNumber": "200",
      "areaId": 1,
      "lotId": 1,
      "status": "Available"
    },
    ...
  ]
}
```

---

## 5️⃣ Contract Endpoints

### Reserve Terrains (Client)
```
POST /contract/reserve
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
{
  "plotIds": [1, 2, 5, 10],
  "proofOfPaymentUrl": "https://exemplo.com/comprovativo.pdf",
  "alternativeContactName": "Maria Silva",
  "alternativeContactPhone": "+258 84 987 6543",
  "relationship": "Mãe"
}

Response (200):
{
  "message": "Reservation created successfully",
  "contractId": 1,
  "contractCode": "CT-20240915-1234",
  "status": "PendingPayment",
  "totalPlots": 4,
  "totalAdhesionValue": 200000,
  "totalInstallmentValue": 192000
}

Errors:
{
  "message": "One or more plots are not available for reservation"
}
```

### Get Pending Contracts (Operator Only)
```
GET /contract/pending
Authorization: Bearer <TOKEN>

Response (200):
[
  {
    "id": 1,
    "contractCode": "CT-20240915-1234",
    "clientName": "João Silva",
    "totalPlots": 4,
    "totalAdhesionValue": 200000,
    "totalInstallmentValue": 192000,
    "paymentPeriodMonths": 24,
    "status": "PendingPayment",
    "plotNumbers": ["100", "101", "105", "110"],
    "createdAt": "2024-09-15T10:30:00Z"
  }
]
```

### Get Contract Details
```
GET /contract/{contractId}
Authorization: Bearer <TOKEN>

Response (200):
{
  "id": 1,
  "contractCode": "CT-20240915-1234",
  "client": {
    "id": 10,
    "fullName": "João Silva",
    "email": "joao@exemplo.com",
    "phoneNumber": "+258 84 123 4567",
    "role": "Client"
  },
  "totalPlots": 4,
  "totalAdhesionValue": 200000,
  "totalInstallmentValue": 192000,
  "paymentPeriodMonths": 24,
  "status": "PendingPayment",
  "plots": [
    {
      "id": 1,
      "plotNumber": "100",
      "areaId": 1,
      "lotId": 1,
      "status": "Reserved"
    }
  ],
  "alternativeContact": {
    "id": 1,
    "fullName": "Maria Silva",
    "phoneNumber": "+258 84 987 6543",
    "relationship": "Mãe"
  },
  "monthlyPayments": [
    {
      "id": 1,
      "installmentNumber": 1,
      "totalInstallments": 24,
      "monthYear": "2024-10-01T00:00:00Z",
      "expectedValue": 8000,
      "paidValue": 0,
      "status": "Pending"
    }
  ],
  "createdAt": "2024-09-15T10:30:00Z"
}
```

### Approve Contract (Operator Only)
```
POST /contract/{contractId}/approve
Authorization: Bearer <TOKEN>

Response (200):
{
  "message": "Contract approved successfully",
  "contractCode": "CT-20240915-1234",
  "status": "Active"
}

Effects:
- Plot status changes from "Reserved" to "Occupied"
- PDF contrato gerado e armazenado
- Notificação enviada ao cliente (futuro)
```

### Reject Contract (Operator Only)
```
POST /contract/{contractId}/reject
Authorization: Bearer <TOKEN>

Response (200):
{
  "message": "Contract rejected successfully"
}

Effects:
- Plot status returns to "Available"
- Contract status changes to "Cancelled"
```

### Get Contract PDF
```
GET /contract/{contractId}/pdf
Authorization: Bearer <TOKEN>

Response (200):
[PDF Binary Data]
Content-Type: application/pdf
Content-Disposition: attachment; filename="Contract_1.pdf"
```

---

## 6️⃣ Payment Endpoints

### Get My Payments (Client)
```
GET /payment/my-payments
Authorization: Bearer <TOKEN>

Response (200):
[
  {
    "id": 1,
    "installmentNumber": 1,
    "totalInstallments": 24,
    "monthYear": "2024-10-01T00:00:00Z",
    "expectedValue": 8000,
    "paidValue": 0,
    "status": "Pending"
  },
  {
    "id": 2,
    "installmentNumber": 2,
    "totalInstallments": 24,
    "monthYear": "2024-11-01T00:00:00Z",
    "expectedValue": 8000,
    "paidValue": 0,
    "status": "Pending"
  }
]
```

### Submit Payment Proof (Client)
```
POST /payment/submit-proof
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
{
  "paymentId": 1,
  "proofOfPaymentUrl": "https://exemplo.com/proof.pdf"
}

Response (200):
{
  "message": "Payment proof submitted successfully"
}

Effects:
- Payment status changes to "UnderAnalysis"
- Notificação ao operador (futuro)
```

### Validate Payment (Operator Only)
```
POST /payment/{paymentId}/validate
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
8000

Response (200):
{
  "message": "Payment validated successfully"
}

Effects:
- Payment status changes to "Paid"
- paidValue = requestBody
- validatedAt = agora
- Recibo gerado (futuro)
```

### Register Manual Payment (Operator Only)
```
POST /payment/{contractId}/manual-payment
Authorization: Bearer <TOKEN>
Content-Type: application/json

Request:
8000

Response (200):
{
  "message": "Manual payment registered successfully"
}

Effects:
- Próximo payment pendente marcado como "Paid"
- Registra pagamento em dinheiro/POS no escritório
```

---

## ⚠️ Error Responses

### 400 Bad Request
```json
{
  "message": "Invalid request parameters"
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid or expired token"
}
```

### 403 Forbidden
```json
{
  "message": "Insufficient permissions"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An unexpected error occurred"
}
```

---

## 📊 Response Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK - Requisição bem sucedida |
| 201 | Created - Recurso criado |
| 400 | Bad Request - Erro na requisição |
| 401 | Unauthorized - Token inválido/expirado |
| 403 | Forbidden - Acesso negado (role) |
| 404 | Not Found - Recurso não encontrado |
| 500 | Server Error - Erro no servidor |

---

## 🧪 Testing com Postman

Importar collection em `/docs/Novo-Horizonte-API.postman_collection.json`

Variáveis Postman:
```
{{base_url}} = https://localhost:5001/api
{{token}} = [TOKEN do login]
{{area_id}} = 1
{{lot_id}} = 1
{{plot_id}} = 1
{{contract_id}} = 1
```

---

## 🔒 Rate Limiting (Futuro)

Em produção, será implementado:
- 100 requests por minuto por IP
- 1000 requests por dia por utilizador autenticado

---

**Última atualização**: Setembro 2024
