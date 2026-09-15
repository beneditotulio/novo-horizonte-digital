# Troubleshooting Guide - Novo Horizonte Digital

## 🔧 Problemas Comuns e Soluções

### Backend Issues

#### Erro: "Cannot connect to database"
**Causa**: SQL Server não está rodando ou connection string incorreta  
**Solução**:
1. Verificar se SQL Server LocalDB está instalado: `sqllocaldb info`
2. Iniciar LocalDB: `sqllocaldb start mssqllocaldb`
3. Verificar connection string em `appsettings.json`

```bash
# Windows - Verificar se LocalDB está rodando
sqllocaldb query

# Se necessário, criar nova instância
sqllocaldb create mssqllocaldb v15.0
sqllocaldb start mssqllocaldb
```

#### Erro: "Port 5001 already in use"
**Causa**: Outra aplicação usando a porta 5001  
**Solução**:
```bash
# Linux/Mac
lsof -i :5001
kill -9 <PID>

# Windows PowerShell
netstat -ano | findstr :5001
taskkill /PID <PID> /F
```

#### Erro: "Unable to create migrations"
**Causa**: Entity Framework Core não está configurado  
**Solução**:
```bash
cd backend/NovoHorizonteDigital.API
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet ef database update
```

#### JWT Token Expired
**Sintoma**: Login funciona mas ações falham com 401 Unauthorized  
**Solução**:
1. Fazer logout e login novamente
2. Verificar relógio do sistema está sincronizado
3. Aumentar `JWT_EXPIRATION_MINUTES` em appsettings.json

---

### Frontend Issues

#### Erro: "Cannot GET /api/..."
**Causa**: API não está rodando ou URL incorreta  
**Solução**:
1. Verificar se backend está rodando: `dotnet run` na pasta backend
2. Verificar URL em `.env`: `REACT_APP_API_URL=https://localhost:5001/api`
3. Limpar cache do navegador: Ctrl+Shift+Delete

#### Erro: "CORS policy error"
**Causa**: Backend não permite origem do frontend  
**Solução**:
1. Verificar CORS em Program.cs está configurado
2. Adicionar origin do frontend à whitelist:
```csharp
policy.AllowAnyOrigin()  // Desenvolvimento
// OU
policy.WithOrigins("http://localhost:3000")  // Produção
```

#### Erro: "HTTPS certificate not trusted"
**Causa**: Certificado self-signed do localhost 5001  
**Solução**:
1. Aceitar certificado no navegador (visitar https://localhost:5001)
2. No React, ignorar erro em desenvolvimento (não recomendado em produção)

#### Npm install falha
**Causa**: Node.js versão incompatível  
**Solução**:
```bash
npm install -g npm@latest
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

---

### Database Issues

#### Erro: "Database already exists"
**Solução**:
```bash
# Deletar banco existente
sqlcmd -S (localdb)\mssqllocaldb -Q "DROP DATABASE NovoHorizonteDigital"

# Recriar
dotnet ef database update
```

#### Migration não aplicada
**Solução**:
```bash
# Verificar migrations
dotnet ef migrations list

# Reverter para versão anterior
dotnet ef database update <MigrationName>

# Ou deletar tudo e recriar
dotnet ef database update 0
dotnet ef database update
```

---

### Authentication Issues

#### Erro: "Invalid username or password"
**Checklist**:
- [ ] Email está correto (case-sensitive)
- [ ] Senha está correta (case-sensitive)
- [ ] Usuário está marcado como IsActive = true
- [ ] Banco de dados foi inicializado com DbInitializer

#### Erro: "Unauthorized" em tudo após login
**Causa**: Token JWT de revezamento incorreto  
**Solução**:
```javascript
// No browser console, limpar token
localStorage.removeItem('authToken');
// Fazer login novamente
```

---

### PDF Generation Issues

#### PDF não gera ou gera vazio
**Causa**: iText7 não instalado ou dados incompletos  
**Solução**:
1. Verificar iText7 está em packages:
```bash
dotnet add package itext7
```
2. Garantir Contract tem todos os dados carregados (Include)
3. Checar logs para exceções

#### PDF muito lento (> 3 segundos)
**Causa**: N+1 query ou dados muito grandes  
**Solução**:
1. Verificar se DbContext está eager-loading:
```csharp
var contract = await _context.Contracts
    .Include(c => c.Client)
    .Include(c => c.ContractPlots)
        .ThenInclude(cp => cp.Plot)
```
2. Implementar caching em DbContext
3. Usar async/await corretamente

---

### Performance Issues

#### Sistema lento ao criar intervalo grande (1000+ terrenos)
**Nota**: Esperado para primeiras vezes (índices sendo criados)  
**Otimizações**:
1. Usar `AddRange` em vez de `Add` individual
2. `SaveChangesAsync` uma única vez
3. Aumentar SQL Server memory allocation

```csharp
// ✅ Correto - Uma save
_context.Plots.AddRange(plots);
await _context.SaveChangesAsync();

// ❌ Errado - Múltiplas saves
foreach(var plot in plots)
{
    _context.Plots.Add(plot);
    await _context.SaveChangesAsync();  // Lento!
}
```

#### Frontend congelado ao carregar muitos terrenos
**Solução**:
1. Implementar paginação
2. Usar virtual scrolling
3. Limitar grid a 50-100 terrenos visíveis

---

### Docker Issues (se usar Docker Compose)

#### Container não inicia
```bash
docker-compose logs api
docker-compose logs web
```

#### Network connectivity error
```bash
docker-compose down
docker-compose up --build
```

#### SQL Server container crashes
Aumentar memória disponível para Docker (Settings > Resources)

---

## 📞 Escalação

Se nenhuma solução funcionar:

1. **Verificar logs**:
   - Backend: `Console` output quando `dotnet run`
   - Frontend: Browser DevTools (F12)
   - Database: SQL Server Management Studio

2. **Coletar informações**:
   - SO, versão .NET, versão Node.js
   - Mensagem de erro completa
   - Stack trace

3. **Contacto**:
   - dev@novo-horizonte-digital.com
   - +258 21 306 000

---

## 🔍 Debugging Tips

### Backend Debugging
```csharp
// Adicionar logs
logger.LogInformation("Contract ID: {contractId}", contractId);
logger.LogError("Error occurred: {error}", ex.Message);

// Usar breakpoints no Visual Studio
// Usar Watch window para inspecionar variáveis
```

### Frontend Debugging
```javascript
// Console logs
console.log('Selected plots:', selectedPlots);
console.error('API Error:', error.response.data);

// React DevTools extension
// Redux DevTools (se adicionar Redux)

// Network tab para ver requisições
// Ver resposta JSON completa
```

### Database Debugging
```sql
-- Verificar dados
SELECT * FROM Contracts WHERE Id = 1;
SELECT * FROM Plots WHERE Status = 1;  -- 1 = Reserved

-- Ver índices
EXEC sp_helpindex 'Plots';

-- Performance
SET STATISTICS TIME ON;
SELECT COUNT(*) FROM Plots;
SET STATISTICS TIME OFF;
```

---

**Mantém este guia atualizado conforme novos issues surgem!**
