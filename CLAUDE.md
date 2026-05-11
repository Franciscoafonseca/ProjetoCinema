# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Visão Geral

Plataforma para festivais de cinema online — projeto académico de Engenharia de Software. Solução .NET 10 com dois projetos: backend ASP.NET Core Web API e frontend Blazor WebAssembly. Persistência via Entity Framework Core sobre SQLite. Integração com TMDB para importação de filmes.

A linguagem do projeto (código, comentários, commits, README) é português europeu (PT-PT). Mantém esse estilo.

## Comandos comuns

Solução: [OnlineCinemaFestival.slnx](OnlineCinemaFestival.slnx).

```bash
# Build de toda a solução
dotnet build

# Build de um projeto específico
dotnet build OnlineCinemaFestival.Api/OnlineCinemaFestival.Api.csproj
dotnet build OnlineCinemaFestival.Client/OnlineCinemaFestival.Client.csproj

# Correr a API (Terminal 1) — fica em http://localhost:5152
cd OnlineCinemaFestival.Api && dotnet run
# Swagger em http://localhost:5152/swagger

# Correr o frontend (Terminal 2) — fica em http://localhost:5257 / https://localhost:7049
cd OnlineCinemaFestival.Client && dotnet run
```

A API precisa estar a correr para o frontend funcionar — não há mock no cliente.

### Base de dados (SQLite, `festival.db`)

```bash
# Criar/atualizar a BD a partir das migrations
cd OnlineCinemaFestival.Api && dotnet ef database update

# Criar nova migration
cd OnlineCinemaFestival.Api && dotnet ef migrations add NomeDaMigration
```

`festival.db` está em `.gitignore` (cada developer cria a sua localmente). As migrations vão para o repositório.

## Arquitetura — Backend (`OnlineCinemaFestival.Api`)

Fluxo em camadas: **Controller → Service (interface) → Repository (interface) → AppDbContext → SQLite**.

- [Controllers/](OnlineCinemaFestival.Api/Controllers/) — endpoints HTTP, finos. Cada controller injeta apenas a interface do service correspondente.
- [Services/](OnlineCinemaFestival.Api/Services/) — regras de negócio. Cada service tem `IXxxService` + `XxxService`. Registados em [Program.cs](OnlineCinemaFestival.Api/Program.cs) como `AddScoped`.
- [Repositories/](OnlineCinemaFestival.Api/Repositories/) — acesso à BD via EF Core. Cada repository tem `IXxxRepository` + `XxxRepository` e expõe `SaveChangesAsync()` separadamente (o service controla quando faz commit).
- [DTOs/](OnlineCinemaFestival.Api/DTOs/) — objetos de transporte; **nunca expor entidades EF diretamente**. Convenção: `XxxReadDto` para output, `XxxCreateDto` para input. Mapeamento centralizado em [Mappers/](OnlineCinemaFestival.Api/Mappers/) (estáticos).
- [Models/](OnlineCinemaFestival.Api/Models/) — entidades EF. `Filme.Popularidade` é uma propriedade calculada (`Avaliacoes.Count`), não uma coluna.
- [Data/AppDbContext.cs](OnlineCinemaFestival.Api/Data/AppDbContext.cs) — `DbSet`s para `Festivals`, `Filmes`, `Avaliacoes`, `Comentarios`.

### TMDB

[TmdbService](OnlineCinemaFestival.Api/Services/TmdbService.cs) chama a API externa do TMDB. Requer config em `appsettings.Development.json` (não vai para o repo):

```json
{ "Tmdb": { "Token": "<bearer>", "BaseUrl": "https://api.themoviedb.org/3/" } }
```

Os posters TMDB são prefixados com `https://image.tmdb.org/t/p/w500` no mapper.

### CORS

Em [Program.cs](OnlineCinemaFestival.Api/Program.cs) há uma whitelist explícita de origens (`localhost:5257`, `7049`, `5000-5002`, `7002`). **Se mudares a porta do cliente, tens de adicionar à whitelist** ou as chamadas falham silenciosamente no browser.

## Arquitetura — Frontend (`OnlineCinemaFestival.Client`)

Blazor WebAssembly. Estrutura:

- [Pages/](OnlineCinemaFestival.Client/Pages/) — rotas (`@page`). `Home`, `Catalogo`, `DetalhesFilme`.
- [Components/](OnlineCinemaFestival.Client/Components/) — componentes reutilizáveis (`FilmeCard`) e classes base.
- [Services/](OnlineCinemaFestival.Client/Services/) — clientes HTTP que chamam a API. Registados em [Program.cs](OnlineCinemaFestival.Client/Program.cs).
- [Models/](OnlineCinemaFestival.Client/Models/) — DTOs do lado cliente. **Espelham exatamente os `ReadDto` da API** (mesmos nomes, mesmos tipos), para desserialização direta com `GetFromJsonAsync`.
- [Layout/](OnlineCinemaFestival.Client/Layout/) — `MainLayout` e `NavMenu`.
- CSS isolado por componente: cada `Foo.razor` tem `Foo.razor.css` ao lado. Estilos globais em [wwwroot/css/app.css](OnlineCinemaFestival.Client/wwwroot/css/app.css).

### Padrão Template Method para páginas que carregam dados

[FilmePageBase](OnlineCinemaFestival.Client/Components/FilmePageBase.cs) é uma classe abstrata que centraliza o ciclo `_loading`/`_erro` num `try/catch/finally` em `OnInitializedAsync`. Páginas com I/O herdam e implementam apenas `CarregarDadosAsync()`. **Quando criares novas páginas que carregam dados, herda desta classe** em vez de duplicar o boilerplate.

### `HttpClient.BaseAddress`

[Program.cs](OnlineCinemaFestival.Client/Program.cs) tem o `BaseAddress` da API hardcoded para `http://localhost:5152/`. Se a API mudar de porta, atualiza aqui.

### Endpoints do catálogo

O cliente usa `CatalogoController` (em vez de `FilmesController`) para todas as listagens e detalhes:

- `GET /api/catalogo` — lista filmes com filtros server-side: `?pesquisa=`, `?genero=`, `?ordenarPor=` (1=Título, 2=Popularidade, 3=Classificação, 4=DataLançamento), `?descendente=`, `?festivalId=`. Todos os parâmetros são opcionais.
- `GET /api/catalogo/filmes/{id}` — detalhe de um filme por id.
- Ambos são `[AllowAnonymous]` — não precisam de token JWT.

`GET /api/filmes` (FilmesController) é usado apenas para importação TMDB (área do dono dos filmes).

### `_Imports.razor`

[_Imports.razor](OnlineCinemaFestival.Client/_Imports.razor) já importa `Models`, `Services` e `Components` — não é preciso `@using` em cada página.

## Convenções

- **Nomes em PT-PT.** Variáveis, métodos, classes, comentários, mensagens de UI — tudo em português. Não traduzir para inglês.
- **DTOs cliente espelham DTOs API.** Se um `ReadDto` no servidor mudar, o `Dto` correspondente no cliente tem de mudar à mesma — caso contrário a desserialização silenciosamente devolve nulls/defaults.
- **Não criar novos DTOs Filme com nomes diferentes do servidor** (`UrlCartaz` vs `CapaUrl` causou retrabalho no passado). Usa exatamente os mesmos nomes.
- Migrations EF têm nomes descritivos em PT (ex.: `AdiconeiFilmeComentarioEAvaliacao`, `AtualizarModeloFilmeTmdbComValoresNulos`).

## Estado e divisão de trabalho

Projeto em desenvolvimento ativo, dividido por pessoas — cada uma é dona de uma fatia (frontend principal, filmes/TMDB, comentários/avaliações, festivais). Antes de mexer numa área, vê os commits recentes (`git log --oneline -20`) para perceber quem está a trabalhar onde.