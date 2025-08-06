# 📝 API To-Do

API RESTful desenvolvida em ASP.NET Core com autenticação JWT e banco de dados SQLite. Permite o gerenciamento de tarefas com proteção de rotas e controle de usuários.

---

## 🚀 Funcionalidades

- Registro e login de usuários com JWT
- CRUD de tarefas (protegido por autenticação)
- Validações básicas
- Documentação via Swagger
- Banco de dados em SQLite

---

## 🛠️ Tecnologias utilizadas

- [.NET 8](https://dotnet.microsoft.com/en-us/)
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT (Json Web Token)
- Swagger (Swashbuckle)

---

## 📦 Estrutura de pastas

```bash
src/ApiToDo
│
├── Controllers/         # Controllers da API (Auth e Tarefas)
├── Data/                # DbContext e configuração EF
├── Models/              # Classes de domínio (Usuario, Tarefa)
├── Services/            # Lógica de geração de tokens JWT
├── Migrations/          # Migrations geradas pelo EF
├── appsettings.json     # Configurações do projeto
└── Program.cs           # Configuração da aplicação
```

---

## 🔐 Autenticação via JWT

1. Registre um usuário em: POST /api/auth/registrar

2.  Faça login em: POST /api/auth/login e copie o token JWT

3.  No Swagger, clique em Authorize e cole:
```bash
Bearer SEU_TOKEN
```
4. Acesse rotas protegidas como /api/tarefas

---

## 💻 Como executar localmente
### **Pré-requisitos:**

-  [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)

-  (Opcional) [Visual Studio Code](https://code.visualstudio.com/)

### **Passos:**

**1. Navegue até a pasta da API**
```bash
cd src/ApiToDo
```
**2. Restaurar pacotes**
```bash
dotnet restore
```
**3.  Rodar migração (cria o database.db)**
```bash
dotnet ef database update
```
**4.  Executar a API**
```bash
dotnet run
```
**Acesse no navegador: http://localhost:5152/swagger**

```
🐳 Docker (em breve)

O projeto pode ser executado com Docker.

    Em breve será adicionado um Dockerfile e docker-compose.yml.

📌 Próximos passos

Adicionar testes com xUnit

Criar pipeline CI/CD no Azure DevOps

Dockerizar o projeto

    Criar frontend (React, Blazor ou outro)

📄 Licença

Este projeto está sob a licença MIT.
Sinta-se livre para usar, estudar e melhorar! 🚀