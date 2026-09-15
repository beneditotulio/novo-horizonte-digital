# 🏢 Novo Horizonte Digital — Sistema de Informação e Gestão Imobiliária

O **Novo Horizonte Digital** é uma plataforma web desenvolvida para a **Yayessa Holdings, Lda.** e a **Cooperativa de Habitação Novo Horizonte**. O sistema automatiza todo o fluxo de pré-inscrição, seleção e reserva de terrenos no **Projecto Habitacional Novo Horizonte** (Katembe / N'sime), eliminando processos manuais, reduzindo filas presenciais e acelerando a emissão de contratos.

---

## 📌 Principais Funcionalidades

### 👑 Módulo Administrador
- **Gestão de Áreas e Categorias:** Modelação de áreas habitacionais (Terreno Jovem 15x30, Área-2 Executivo 20x30, Área-1 20x40, Área+ 20x40), definindo preços, taxas de adesão, prestações e prazos em meses.
- **Criação Flexível de Terrenos:** Cadastro individual de terrenos ou geração automática em lote por intervalo/range numérico (ex: Terrenos Nº 600 a 650 para o Lote 01).
- **Gestão de Utilizadores e Permissões:** Gestão de perfis de utilizadores (Administrador, Operador Comercial, Cliente).
- **Relatórios e Indicadores:** Acompanhamento da taxa de ocupação dos lotes, receita arrecadada e relatórios de inadimplência.

### 👔 Módulo Operador Comercial / Atendimento
- **Validação Documental:** Análise rápida de cópias digitais do BI, NUIT e comprovativos de pagamento.
- **Motor de Emissão Automática de Contratos (PDF Engine):** Geração da minuta consolidada do contrato em formato PDF em menos de 3 segundos, incluindo múltiplos terrenos e cláusulas contratuais.
- **Impressão Instantânea:** Impressão de 2 vias do contrato já preenchido diretamente no escritório.
- **Gestão Financeira Presencial:** Registo e baixa manual de pagamentos em numerário ou POS.

### 👤 Módulo Cliente / Membro da Cooperativa
- **Mapa e Escolha Livre de Terrenos:** Consulta pública e visual de áreas, lotes e números de terrenos disponíveis.
- **Reserva Múltipla Dinâmica:** Possibilidade de selecionar **mínimo 1 terreno, sem limite máximo** por transação.
- **Pré-inscrição Remota:** Formulário integrado para dados pessoais, carregamento de documentos e contacto familiar alternativo.
- **Portal do Cliente:** Download do contrato assinado/gerado e acompanhamento do plano de amortização e histórico de pagamentos.
- **Submissão Mensal:** Upload fácil de comprovativos das prestações mensais via web ou telemóvel.

---

## 🛠️ Tecnologias Recomendadas (Stack)

- **Backend:** C# (.NET Core / ASP.NET Core MVC) ou Node.js (TypeScript)
- **Database:** PostgreSQL / SQL Server / MySQL
- **Frontend:** React / Next.js ou ASP.NET Core Razor Views
- **Geração de PDF:** QuestPDF / Puppeteer / iTextSharp
- **Estilização:** Tailwind CSS / Bootstrap

---

## 🚀 Como Executar o Projeto Localmente

### Pré-requisitos
- **SDK / Runtime:** .NET 8.0 SDK (ou Node.js v18+)
- **Base de Dados:** PostgreSQL / SQL Server
- **Git**

### Passo a Passo

1. **Clonar o Repositório:**
   ```bash
   git clone [https://github.com/beneditotulio/novo-horizonte-digital.git](https://github.com/beneditotulio/novo-horizonte-digital.git)
   cd novo-horizonte-digital