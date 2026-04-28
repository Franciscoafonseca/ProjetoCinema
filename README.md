# Online Cinema Festival

Projeto desenvolvido no âmbito da Unidade Curricular de Engenharia de Software.

O objetivo do sistema é criar uma plataforma online para festivais de cinema, permitindo a gestão de festivais, catálogo de filmes, sessões digitais, acessos, compras e funcionalidades sociais associadas à experiência de um festival de cinema online.

---

## Arquitetura do Projeto

Este projeto segue o **Caminho B — Web API + Blazor WebAssembly**.

A solução está dividida em dois projetos principais:

```text
ProjetoCinema/
│
├── OnlineCinemaFestival.Api/       # Backend ASP.NET Core Web API
├── OnlineCinemaFestival.Client/    # Frontend Blazor WebAssembly
├── OnlineCinemaFestival.slnx       # Solution
├── .gitignore
└── README.md
```

---

## Tecnologias Utilizadas

- .NET 10
- ASP.NET Core Web API
- Blazor WebAssembly
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- Git e GitHub

---

## Padrões e Organização

O backend está organizado em camadas, seguindo uma estrutura preparada para evolução e manutenção:

```text
OnlineCinemaFestival.Api/
│
├── Controllers/     # Endpoints HTTP da API
├── Data/            # AppDbContext e configuração da base de dados
├── Models/          # Entidades do domínio
├── Repositories/    # Acesso à base de dados
├── Services/        # Lógica de negócio
├── Migrations/      # Histórico de alterações da base de dados
└── Program.cs       # Configuração da aplicação
```

Fluxo principal da API:

```text
Controller -> Service -> Repository -> AppDbContext -> SQLite
```

Responsabilidades:

- **Controllers**: recebem pedidos HTTP e devolvem respostas.
- **Services**: concentram regras de negócio e validações.
- **Repositories**: centralizam o acesso à base de dados.
- **Models**: representam as entidades principais do sistema.
- **Data/AppDbContext**: configura o Entity Framework Core e a ligação à base de dados.

---

## Funcionalidades Iniciais

Nesta fase inicial, o projeto inclui:

- Criação da solução com API e frontend separados.
- Configuração da Web API.
- Configuração do Blazor WebAssembly.
- Integração com Entity Framework Core.
- Utilização de SQLite como base de dados local.
- Criação da entidade `Festival`.
- Criação de migration inicial.
- Endpoints REST para gestão inicial de festivais.
- Página frontend para consulta de dados através da API.

---

## Funcionalidades Planeadas

De acordo com o enunciado do projeto, a plataforma deverá evoluir para incluir:

### Gestão de Utilizadores

- Registo de utilizadores.
- Autenticação.
- Gestão de perfil.
- Perfis públicos.
- Sistema de seguidores.

### Gestão de Festivais

- Criação de festivais.
- Edição de festivais.
- Remoção de festivais.
- Consulta de festivais.
- Associação de filmes a festivais.

### Catálogo de Filmes

- Criação e gestão de filmes.
- Consulta de detalhes de filmes.
- Pesquisa por título ou palavra-chave.
- Filtragem por género, festival, popularidade ou classificação.
- Ordenação de resultados.

### Sessões

- Criação de sessões associadas a filmes.
- Sessões em horário fixo.
- Estreias.
- Janelas de acesso.
- Consulta de sessões disponíveis.

### Sistema de Acessos

- Bilhete por sessão.
- Passe diário.
- Passe completo.
- Aluguer digital com janela temporal.

### Carrinho e Compras

- Adição de acessos ao carrinho.
- Remoção de itens.
- Validação de regras de negócio.
- Finalização de compra.
- Histórico de compras.

### Visualização de Conteúdos

- Validação de acesso antes da visualização.
- Player incorporado.
- Reprodução através de URL externo ou ficheiro local.

### Componente Social

- Avaliação de filmes.
- Comentários.
- Reporte de conteúdos inadequados.
- Votação para prémios do público.
- Listas pessoais, como favoritos, vistos e quero ver.

### Recomendações

- Recomendações por género.
- Recomendações por popularidade.
- Recomendações por avaliação.

---

## Como Executar o Projeto

### Pré-requisitos

Antes de correr o projeto, é necessário ter instalado:

- .NET SDK 10
- Git
- Visual Studio Code ou Visual Studio
- Ferramenta do Entity Framework Core

Para confirmar a versão do .NET:

```bash
dotnet --version
```

Para instalar ou atualizar a ferramenta do EF Core:

```bash
dotnet tool install --global dotnet-ef
```

ou, caso já esteja instalada:

```bash
dotnet tool update --global dotnet-ef
```

---

## Restaurar Dependências

Na raiz do projeto:

```bash
dotnet restore
```

---

## Criar ou Atualizar a Base de Dados

A base de dados local SQLite não é enviada para o GitHub.

Para criar a base de dados a partir das migrations:

```bash
cd OnlineCinemaFestival.Api
dotnet ef database update
```

Isto irá criar o ficheiro local:

```text
festival.db
```

Este ficheiro está ignorado pelo Git através do `.gitignore`.

---

## Executar a API

Dentro da pasta da API:

```bash
cd OnlineCinemaFestival.Api
dotnet run
```

A API irá indicar no terminal o endereço onde está a correr, por exemplo:

```text
http://localhost:5187
```

O Swagger pode ser acedido em:

```text
http://localhost:5187/swagger
```

O porto pode variar conforme a configuração local.

---

## Executar o Frontend

Num segundo terminal, dentro da pasta do frontend:

```bash
cd OnlineCinemaFestival.Client
dotnet run
```

O frontend Blazor WebAssembly irá indicar no terminal o endereço onde está disponível.

---

## Configuração da Comunicação API - Frontend

O frontend comunica com a API através de `HttpClient`.

No ficheiro:

```text
OnlineCinemaFestival.Client/Program.cs
```

deve existir uma configuração semelhante a:

```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5187")
});
```

O URL deve corresponder ao endereço real onde a API está a correr.

Se a API estiver noutro porto, este valor deve ser atualizado.

---

## Testes Manuais Iniciais

Depois de correr a API, é possível testar os endpoints através do Swagger.

### Criar um festival

Endpoint:

```text
POST /api/festivals
```

Exemplo de JSON:

```json
{
  "name": "Festival Cinema Madeira 2026",
  "description": "Festival online dedicado ao cinema independente e internacional.",
  "startDate": "2026-05-01T00:00:00",
  "endDate": "2026-05-10T00:00:00"
}
```

### Listar festivais

Endpoint:

```text
GET /api/festivals
```

---

## Comandos Úteis

### Build completo

Na raiz do projeto:

```bash
dotnet build
```

### Build apenas da API

```bash
dotnet build OnlineCinemaFestival.Api/OnlineCinemaFestival.Api.csproj
```

### Build apenas do frontend

```bash
dotnet build OnlineCinemaFestival.Client/OnlineCinemaFestival.Client.csproj
```

### Criar nova migration

Dentro da pasta da API:

```bash
dotnet ef migrations add NomeDaMigration
```

Exemplo:

```bash
dotnet ef migrations add AddFilms
```

### Aplicar migrations

Dentro da pasta da API:

```bash
dotnet ef database update
```

---

## Regras para Git

Não devem ser enviados para o GitHub:

```text
bin/
obj/
.vs/
*.db
*.db-shm
*.db-wal
.env
.env.local
```

Devem ser enviados para o GitHub:

```text
Controllers/
Data/
Models/
Repositories/
Services/
Migrations/
Pages/
Layout/
wwwroot/
.csproj
.slnx
README.md
.gitignore
```

A base de dados local `festival.db` não deve ser enviada.

As migrations devem ser enviadas, pois permitem recriar a base de dados noutro computador.

---

## Estado Atual do Projeto

Estado atual da implementação:

```text
Concluído:
- Estrutura inicial da solução
- Projeto Web API
- Projeto Blazor WebAssembly
- Configuração do Entity Framework Core
- Configuração de SQLite
- Entidade Festival
- Repository de Festival
- Service de Festival
- Controller de Festival
- Migration inicial
- Swagger configurado

Em desenvolvimento:
- Entidade Film
- Catálogo de filmes
- Associação de filmes a festivais
- Página frontend para listagem de festivais e filmes
```

---

## Próximos Passos

A ordem recomendada de desenvolvimento é:

```text
1. Finalizar gestão de festivais
2. Criar gestão de filmes
3. Associar filmes a festivais
4. Criar sessões
5. Implementar tipos de acesso
6. Criar carrinho
7. Criar processo de compra
8. Validar acessos
9. Criar player por URL
10. Implementar histórico
11. Adicionar avaliações e comentários
12. Adicionar listas pessoais
13. Implementar recomendações simples
```

---

## Autores

Projeto desenvolvido por estudantes da Licenciatura em Engenharia Informática / Engenharia de Computadores, no âmbito da Unidade Curricular de Engenharia de Software.

---

## Nota

Este projeto ainda se encontra em desenvolvimento académico. Algumas funcionalidades estão em fase inicial ou planeadas para iterações futuras.
