# 🏗️ Sistema Novo Horizonte Digital - Protótipo Funcional

Um **Sistema de Gestão Imobiliária completo** desenvolvido em **ASP.NET Core 8** + **React 18** para automatizar o processo de venda, reserva e gestão de terrenos da Cooperativa de Habitação Novo Horizonte.

## ✨ Características Principais

### ✅ Autenticação & Autorização
- Login seguro com JWT Token
- Controle de acesso baseado em roles (Admin, Operador, Cliente)
- Tokens com expiração de 24 horas

### 📊 Gestão de Áreas e Lotes
- Criação e edição de áreas de habitação (Terreno Jovem, Área Executivo, etc.)
- Definição de valores de adesão, prestações e prazos
- Gestão de lotes associados a cada área

### 🏘️ Criação de Terrenos (Individual e em Intervalo)
- Criar terrenos um a um ou em massa via intervalo numérico
- Exemplo: Criar 51 terrenos de uma só vez (600-650)
- Associação automática a área e lote

### 👁️ Seleção Visual de Terrenos pelo Cliente
- Interface interativa para escolha de terrenos
- Grid visual com números dos terrenos
- Seleção múltipla (mínimo 1, sem limite superior)
- Bloqueio temporário de terrenos durante reserva

### 📋 Fluxo de Reserva Completo
1. Cliente preenche formulário com dados pessoais e contacto alternativo
2. Faz upload de documentos (BI, NUIT, comprovativo)
3. Sistema gera temporáriamente o estado "Reservado"
4. Operador valida documentos e pagamento
5. Operador aprova contrato

### 📄 Geração Automática de Contratos em PDF
- Contrato gerado em **< 3 segundos**
- Incluir todos os dados do cliente e terrenos selecionados
- Pronto para impressão em 2 vias
- Cláusulas legais consolidadas

### 💳 Gestão de Mensalidades
- Plano de amortização automático (12, 15 ou 24 meses)
- Submissão remota de comprovativos pelo cliente
- Validação de pagamentos pelo operador
- Histórico de pagamentos com status (Pendente, Em Análise, Pago, Atrasado)

### 📊 Dashboards Específicos por Perfil
- **Admin**: Cadastro de áreas, lotes e terrenos; relatórios
- **Operador**: Validação de reservas, emissão de contratos, gestão de pagamentos
- **Cliente**: Seleção de terrenos, histórico de reservas, pagamento de mensalidades

---

## 🏛️ Arquitetura do Projeto

```
novo-horizonte-digital/
├── backend/
│   └── NovoHorizonteDigital.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── AreaController.cs
│       │   ├── LotController.cs
│       │   ├── PlotController.cs
│       │   ├── ContractController.cs
│       │   └── PaymentController.cs
│       ├── Models/
│       │   ├── User.cs
│       │   ├── Area.cs
│       │   ├── Lot.cs
│       │   ├── Plot.cs
│       │   ├── Contract.cs
│       │   ├── ContractPlot.cs
│       │   ├── AlternativeContact.cs
│       │   └── MonthlyPayment.cs
│       ├── Services/
│       │   ├── AuthService.cs
│       │   ├── PlotService.cs
│       │   ├── ContractService.cs
│       │   └── PdfService.cs
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   └── DbInitializer.cs
│       ├── DTOs/
│       │   └── RequestResponse.cs
│       ├── Program.cs
│       ├── appsettings.json
│       └── NovoHorizonteDigital.API.csproj
│
├── frontend/
│   └── novo-horizonte-app/
│       ├── src/
│       │   ├── components/
│       │   │   ├── Login.js
│       │   │   ├── AdminDashboard.js
│       │   │   ├── OperatorDashboard.js
│       │   │   └── ClientDashboard.js
│       │   ├── services/
│       │   │   └── ApiService.js
│       │   ├── App.js
│       │   └── index.js
│       ├── public/
│       │   └── index.html
│       └── package.json
│
└── README.md
```

---

## 🚀 Instalação e Configuração

### Pré-requisitos
- **.NET SDK 8.0** ou superior
- **Node.js 18+** e npm
- **SQL Server** ou SQL Server Express LocalDB
- **Visual Studio Code** ou Visual Studio 2022

### Backend (ASP.NET Core)

**1. Navegar para a pasta do backend:**
```bash
cd backend/NovoHorizonteDigital.API
```

**2. Restaurar dependências:**
```bash
dotnet restore
```

**3. Aplicar migrations do banco de dados:**
```bash
dotnet ef database update
```

**4. Executar o servidor:**
```bash
dotnet run
```

O backend estará disponível em: `https://localhost:5001`

### Frontend (React)

**1. Navegar para a pasta do frontend:**
```bash
cd frontend/novo-horizonte-app
```

**2. Instalar dependências:**
```bash
npm install
```

**3. Criar arquivo `.env` (opcional):**
```
REACT_APP_API_URL=https://localhost:5001/api
```

**4. Executar a aplicação:**
```bash
npm start
```

O frontend estará disponível em: `http://localhost:3000`

---

## 🔐 Credenciais de Demo

### Admin
- **Email**: admin@novo-horizonte.com
- **Senha**: Admin@123

### Operador Comercial
- **Email**: operador@novo-horizonte.com
- **Senha**: Operator@123

### Cliente (criar novo via interface de registro)
- Qualquer email e senha

---

## 📚 Fluxos Principais

### 1️⃣ Administrador - Cadastro de Áreas e Terrenos

```
1. Fazer login com credenciais de admin
2. Ir para "Dashboard do Administrador"
3. Aba "Gestão de Áreas"
   - Preencher formulário e criar área
4. Aba "Gestão de Lotes"
   - Selecionar área e criar lote
5. Aba "Gestão de Terrenos"
   - Opção 1: Criar terreno individual
   - Opção 2: Criar em intervalo (Ex: 100-150 cria 51 terrenos)
```

### 2️⃣ Cliente - Reservar Terreno(s)

```
1. Fazer login ou registrar-se como cliente
2. Ir para "Dashboard do Cliente"
3. Selecionar Categoria de Terreno
4. Selecionar Lote
5. Clicar nos números de terrenos desejados (grid visual)
6. Clicar "Reservar Terreno(s)"
7. Preencher formulário:
   - Contacto alternativo (familiar)
   - URL do comprovativo de pagamento
8. Enviar reserva
```

### 3️⃣ Operador - Validar e Aprovar Contrato

```
1. Fazer login com credenciais de operador
2. Ir para "Dashboard do Operador"
3. Ver lista de "Reservas Pendentes de Análise"
4. Clicar "Ver Detalhes"
5. Revisar:
   - Dados do cliente
   - Terrenos selecionados
   - Informações financeiras
   - Contacto alternativo
6. Clicar "Aprovar Contrato" ou "Rejeitar"
7. (Opcional) Clicar "Descarregar PDF" para imprimir
```

### 4️⃣ Cliente - Pagar Mensalidades

```
1. Fazer login como cliente
2. Ir para "Dashboard do Cliente"
3. Ver seção "Minhas Mensalidades"
4. Para cada parcela pendente:
    - Fazer pagamento
    - Fazer upload do comprovativo
    - Operador valida e confirma
```

---

## 🔌 Endpoints da API

### Autenticação
```
POST /api/auth/login
POST /api/auth/register
```

### Áreas
```
GET /api/area
GET /api/area/{id}
POST /api/area (Admin)
```

### Lotes
```
GET /api/lot/area/{areaId}
POST /api/lot (Admin)
```

### Terrenos
```
GET /api/plot/available/lot/{lotId}
GET /api/plot/area/{areaId}/lot/{lotId}
POST /api/plot (Admin)
POST /api/plot/range (Admin)
```

### Contratos
```
POST /api/contract/reserve (Cliente)
GET /api/contract/pending (Operador)
GET /api/contract/{id}
POST /api/contract/{id}/approve (Operador)
POST /api/contract/{id}/reject (Operador)
GET /api/contract/{id}/pdf
```

### Pagamentos
```
GET /api/payment/my-payments (Cliente)
POST /api/payment/submit-proof (Cliente)
POST /api/payment/{id}/validate (Operador)
POST /api/payment/{contractId}/manual-payment (Operador)
```

---

## 📦 Dependências Principais

### Backend
- **Entity Framework Core 8.0** - ORM
- **JWT Bearer** - Autenticação
- **BCrypt.Net-Next** - Hash de senha
- **iText7** - Geração de PDF
- **Swagger/Swashbuckle** - Documentação API

### Frontend
- **React 18** - UI Framework
- **React Router v6** - Navegação
- **Axios** - Cliente HTTP
- **React Bootstrap** - Componentes UI
- **React Toastify** - Notificações
- **JWT Decode** - Decodificação de tokens

---

## 🎯 Requisitos Funcionais Implementados

| RF | Descrição | Status |
|---|---|---|
| **RF01** | Gestão de Utilizadores | ✅ |
| **RF02** | Autenticação de Utilizadores | ✅ |
| **RF03** | Gestão de Perfis e Permissões | ✅ |
| **RF04** | Cadastro de Áreas de Loteamento | ✅ |
| **RF05** | Gestão de Lotes | ✅ |
| **RF06** | **Criação de Terrenos (Individual e em Intervalo)** | ✅ |
| **RF07** | Consulta e Escolha Visual de Terrenos | ✅ |
| **RF08** | Formulário de Pré-Inscrição | ✅ |
| **RF09** | Submissão de Documentos Digitais | ✅ |
| **RF10** | **Selecção Múltipla de Terrenos** | ✅ |
| **RF11** | Bloqueio Temporário Múltiplo | ✅ |
| **RF12** | Validação de Reservas | ✅ |
| **RF13** | **Geração Automática de Contrato em PDF** | ✅ |
| **RF14** | Emissão e Impressão de Contratos | ✅ |
| **RF15** | Plano de Amortização Financeiro | ✅ |
| **RF16** | Submissão de Comprovativos de Mensalidades | ✅ |
| **RF17** | Validação de Pagamentos | ✅ |
| **RF18** | Registo Manual de Pagamentos | ✅ |
| **RF19** | Consulta do Estado de Conta | ✅ |
| **RF20** | Geração de Relatórios (Base) | 🔄 |
| **RF21** | Notificação de Estado (Base) | 🔄 |

---

## 🚧 Melhorias Futuras

1. **Email Notifications** - Notificar cliente via email sobre aprovação/rejeição
2. **SMS Alerts** - Lembrete de pagamento via SMS
3. **Integração M-Pesa** - Payment gateway para Moçambique
4. **Report Generator** - Dashboard com gráficos de receitas e ocupação
5. **Mobile App** - Aplicação nativa para iOS/Android
6. **2FA** - Autenticação de dois fatores
7. **Audit Logs** - Rastreamento completo de ações
8. **Backup Automático** - Sistema de backup diário

---

## 📞 Suporte & Contacto

Para dúvidas ou sugestões sobre o protótipo:
- **Email**: dev@novo-horizonte-digital.com
- **Telefone**: +258 21 306 000

---

## 📄 Licença

Propriedade da Cooperativa de Habitação Novo Horizonte © 2024

---

**Versão**: 1.0.0 (Protótipo Funcional)  
**Data**: Setembro 2024  
**Desenvolvido por**: .NET Senior Developer
