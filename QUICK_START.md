## ⚡ QUICK START - Novo Horizonte Digital (5 MINUTOS)

### PRÉ-REQUISITOS
```bash
✓ .NET SDK 8.0+
✓ Node.js 18+
✓ SQL Server ou LocalDB
✓ VS Code ou Visual Studio
```

---

## STEP 1: Clonar Repositório
```bash
git clone https://github.com/seu-username/novo-horizonte-digital.git
cd novo-horizonte-digital
```

---

## STEP 2: Iniciar Backend (Terminal 1)
```bash
cd backend/NovoHorizonteDigital.API

# Restaurar dependências
dotnet restore

# Criar e popular banco de dados
dotnet ef database update

# Initializar com dados de demo
# (Automático via DbInitializer no Program.cs)

# Executar servidor
dotnet run

# ✅ Servidor rodando em: https://localhost:5001
# ✅ Swagger em: https://localhost:5001/swagger/index.html
```

---

## STEP 3: Iniciar Frontend (Terminal 2)
```bash
cd frontend/novo-horizonte-app

# Instalar dependências
npm install

# Executar aplicação
npm start

# ✅ App abrirá em: http://localhost:3000
```

---

## STEP 4: Fazer Login (Demo)

### Como Admin
1. Abrir http://localhost:3000
2. **Email**: admin@novo-horizonte.com
3. **Senha**: Admin@123
4. Clicar "Entrar"
5. ✅ Você está em "Dashboard do Administrador"

### Como Operador
1. Sair (canto superior direito)
2. **Email**: operador@novo-horizonte.com
3. **Senha**: Operator@123
4. ✅ Você está em "Dashboard do Operador"

### Como Cliente (Novo)
1. Sair
2. (Criar novo cliente via interface de registro)
3. Preencher dados e clicar "Register"
4. ✅ Você está em "Dashboard do Cliente"

---

## STEP 5: Testar Funcionalidades Críticas

### A. Admin - Criar Terrenos em Intervalo (2 MINUTOS)
```
1. Dashboard Admin
2. Aba "Gestão de Terrenos" → "Criar Terrenos em Intervalo"
3. Preencher:
   - Área: "Terreno Jovem"
   - Lote: "Lote 01"
   - Número Inicial: 600
   - Número Final: 650
4. Clicar "Criar Intervalo"
5. ✅ Toast verde: "51 plots created successfully"
6. ⏱️ Tempo: ~2 segundos
```

### B. Cliente - Selecionar Terrenos (2 MINUTOS)
```
1. Login como cliente
2. Dashboard Cliente → "Selecione Terrenos"
3. Categoria: "Terreno Jovem (15x30)"
4. Lote: "Lote 01"
5. ✅ Grid com 600-650 aparece
6. Clicar em: 600, 601, 610, 620
7. ✅ 4 terrenos selecionados (fundo azul)
8. Clicar "Reservar Terreno(s)"
9. Preencher Modal:
   - Contacto: Maria Silva
   - Telefone: +258 84 987 6543
   - Parentesco: Mãe
   - URL: https://exemplo.com/prova.pdf
10. Clicar "Enviar Reserva"
11. ✅ Toast verde: "Reserva criada com sucesso!"
```

### C. Operador - Validar PDF (1 MINUTO)
```
1. Login como operador
2. Dashboard Operador → "Reservas Pendentes"
3. ✅ Vê a reserva do cliente
4. Clicar "Ver Detalhes"
5. Revisar tabs: Cliente, Imóvel, Financeiro, Contacto
6. Clicar "Descarregar PDF"
7. ✅ PDF abre com contrato completo
8. Voltar modal, clicar "Aprovar Contrato"
9. ✅ Toast verde: "Contrato aprovado com sucesso!"
```

---

## 🎯 RESULTADO ESPERADO

✅ 51 terrenos criados em < 2 segundos  
✅ Cliente seleciona visualmente 4 terrenos  
✅ Reserva feita com bloqueio instantâneo  
✅ PDF gerado em < 3 segundos  
✅ Operador aprova em 1 segundo  
✅ Sistema inteiro funciona perfeitamente  

---

## 📊 DASHBOARD VIEWS

### Admin Dashboard
```
Abas:
  → Gestão de Áreas (CRUD)
  → Gestão de Lotes (CRUD)
  → Gestão de Terrenos
      - Individual
      - Em Intervalo (STAR ⭐)
```

### Cliente Dashboard
```
Coluna 1:
  → Seleção Visual de Terrenos
    - Categoria dropdown
    - Lote dropdown
    - Grid interativa (600-650)
    - Botão "Reservar"

Coluna 2:
  → Minhas Mensalidades
    - Tabela com parcelas
    - Status (Pendente/Pago/etc)
    - Valores MT
```

### Operador Dashboard
```
Tabela: Reservas Pendentes
  - Código Contrato
  - Nome Cliente
  - Qtd Terrenos
  - Valor Adesão
  - Data
  - Botão "Ver Detalhes"

Modal "Ver Detalhes":
  - Tabs: Cliente, Imóvel, Financeiro, Contacto
  - Botões: Descarregar PDF, Aprovar, Rejeitar
```

---

## 🔧 TROUBLESHOOTING RÁPIDO

### Erro: "Cannot connect to database"
```bash
# Windows - Iniciar LocalDB
sqllocaldb start mssqllocaldb

# Ou conferir connection string em appsettings.json
ConnectionStrings:DefaultConnection
```

### Erro: "Port 5001 already in use"
```bash
# Windows PowerShell
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Ou mudar porta em launchSettings.json
```

### Erro: "CORS policy error"
```bash
# Já configurado em Program.cs
# Se problema persistir, verificar:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
```

### Frontend não conecta API
```bash
# Verificar se backend está rodando: https://localhost:5001
# Verificar .env do React: REACT_APP_API_URL=https://localhost:5001/api
# Limpar cache: Ctrl+Shift+Delete
```

---

## 📚 DOCUMENTAÇÃO COMPLETA

Para informações detalhadas:
- **GUIA_COMPLETO.md** - Arquitetura completa
- **API_DOCUMENTATION.md** - Todos endpoints com exemplos
- **GUIA_DEMONSTRACAO.md** - Script detalhado
- **TROUBLESHOOTING.md** - Resolução de problemas

---

## ⏱️ TIMELINE ESPERADO

```
00:00 - 00:05  →  Setup Backend + Frontend
00:05 - 00:07  →  Admin: Criar intervalo de terrenos
00:07 - 00:10  →  Cliente: Selecionar e reservar terrenos
00:10 - 00:12  →  Operador: Aprovar e gerar PDF
00:12 - 00:15  →  Q&A e Discussão
```

---

## 🔐 DADOS DE LOGIN

```
┌──────────────────────────────────────────────────────────────┐
│ ADMIN                                                        │
├──────────────────────────────────────────────────────────────┤
│ Email: admin@novo-horizonte.com                             │
│ Senha: Admin@123                                             │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ OPERADOR COMERCIAL                                           │
├──────────────────────────────────────────────────────────────┤
│ Email: operador@novo-horizonte.com                          │
│ Senha: Operator@123                                          │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ CLIENTE (Novo)                                               │
├──────────────────────────────────────────────────────────────┤
│ Criar via interface "Register"                              │
│ Email: qualquer@email.com                                    │
│ Senha: qualquer senha                                        │
└──────────────────────────────────────────────────────────────┘
```

---

## 🚀 URLS IMPORTANTES

```
Backend API
├── Swagger: https://localhost:5001/swagger/index.html
├── Health:  https://localhost:5001/health
└── API:     https://localhost:5001/api/

Frontend
├── App:     http://localhost:3000
├── Login:   http://localhost:3000/login
└── Logout:  localStorage.removeItem('authToken')

Database
├── LocalDB: (localdb)\mssqllocaldb
├── Database: NovoHorizonteDigital
└── SSMS:    Connect to (localdb)\mssqllocaldb
```

---

## 📝 NOTAS IMPORTANTES

- ✅ Dados de demo já estão no banco (admin, operador, áreas, lotes, terrenos)
- ✅ Sistema de arquivos preparado para upload futuro
- ✅ Todas as mensalidades geradas automaticamente
- ✅ PDF consolidado com cliente + terrenos + cláusulas
- ✅ Bloqueio de terrenos implementado
- ✅ Dashboard com abas por funcionalidade

---

## 🎉 READY TO GO!

```
Backend:   ✅ https://localhost:5001
Frontend:  ✅ http://localhost:3000
Database:  ✅ NovoHorizonteDigital
Demo Data: ✅ Inicializado

STATUS: 🟢 PRONTO PARA DEMONSTRAÇÃO
```

**Tempo para Start**: ~5 minutos  
**Tempo para Demo Completa**: ~30 minutos  
**Impressão Esperada**: Profissional e Funcional ✅

---

Se tiver dúvidas, consultar:
- **TROUBLESHOOTING.md** para problemas
- **API_DOCUMENTATION.md** para endpoints
- **GUIA_COMPLETO.md** para arquitetura completa

🎬 **LET'S GO PRESENT!**
