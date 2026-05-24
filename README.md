# Online Cinema Festival

Plataforma académica para festivais de cinema online — backend ASP.NET Core Web API, frontend Blazor WebAssembly, EF Core com SQLite.

---

## Índice

1. [Descrição](#descrição)
2. [Arquitetura](#arquitetura)
3. [Padrões de design aplicados](#padrões-de-design-aplicados)
4. [Principais funcionalidades](#principais-funcionalidades)
5. [Estrutura de pastas](#estrutura-de-pastas)
6. [Configuração e arranque](#configuração-e-arranque)
7. [Migrations e base de dados](#migrations-e-base-de-dados)
8. [Testes](#testes)
9. [Imagens, logos e uploads](#imagens-logos-e-uploads)
10. [Credenciais demo](#credenciais-demo)
11. [Fluxo de demonstração](#fluxo-de-demonstração)
12. [Limitações conhecidas](#limitações-conhecidas)

---

## Descrição

Sistema de festivais de cinema online que permite a utilizadores registados comprar acessos a sessões, alugar filmes, votar em prémios, criar listas pessoais e participar em comunidades. Um utilizador administrador gere o catálogo, festivais, sessões e prémios.

---

## Arquitetura

```
OnlineCinemaFestival.Api      Backend ASP.NET Core Web API (.NET 10)
OnlineCinemaFestival.Client   Frontend Blazor WebAssembly (.NET 10)
OnlineCinemaFestival.Tests    Testes unitários xUnit (.NET 10)
OnlineCinemaFestival.slnx     Solution
```

**Fluxo no backend:**

```
HTTP Request
    └─► Controller          (recebe HTTP, valida rota/auth, devolve resposta)
            └─► Service     (regras de negócio, orquestração)
                    └─► Repository  (queries EF Core)
                                └─► AppDbContext / SQLite
```

**Princípios aplicados:**

| Princípio | Onde |
|-----------|------|
| SRP | Services com responsabilidade única; controllers finos sem lógica de negócio |
| OCP | Novos tipos de acesso, pagamento ou ordenação entram por novas strategies |
| DIP | Services dependem de interfaces, não de classes concretas |
| ISP | Interfaces pequenas por domínio |
| Repository | Acesso a dados isolado; controllers nunca acedem ao `DbContext` diretamente |

---

## Padrões de design aplicados

### Strategy
- **Validação de acessos** (`Services/AcessoFolder/`): `BilheteSessaoValidacaoStrategy`, `EstrategiaValidacaoPasseDiario`, `ValidacaoPasseCompletoStrategy`, `AluguerDigitalValidacaoStrategy`
- **Criação de acessos** (`Services/`): `EstrategiaCriacaoBilheteSessao`, `EstrategiaCriacaoPasseDiario`, `EstrategiaCriacaoPasseCompleto`, `EstrategiaCriacaoAluguerDigital`
- **Ordenação de catálogo** (`Services/Catalogo/`): `OrdenarPorTituloStrategy`, `OrdenarPorClassificacaoStrategy`, `OrdenarPorPopularidadeStrategy`, `OrdenarPorDataLancamentoStrategy`, `OrdenarPorVisualizacoesStrategy`, `OrdenarPorFestivalStrategy`
- **Pagamentos** (`Services/`): `PagamentoAprovadoSimuladoStrategy`, `PagamentoReferenciaMultibancoStrategy`
- **Recomendações** (`Services/`): `RecomendacaoPorGeneroStrategy`, `RecomendacaoPorAvaliacaoStrategy`, `RecomendacaoPorPopularidadeStrategy`, `RecomendacaoPorPremiosStrategy`

### Factory
- `AcessoAutomaticoFactory` — cria acessos automáticos pós-compra com base no tipo
- `AcessoUtilizadorFactory` — instancia `AcessoUtilizador` a partir de um acesso
- `CompraFactory` — constrói `Compra` a partir de um carrinho validado
- `CatalogoOrdenacaoStrategyFactory` — resolve a strategy de ordenação pelo enum `CatalogoOrdenacao`
- `ValidacaoAcessoStrategyFactory` — resolve a strategy de validação pelo tipo de acesso

### Observer
- **Rewards**: `RewardsObserver`, `RewardsAvaliacaoObserver`, `RewardsComentarioObserver`, `RewardsVisualizacaoObserver`, `RewardsListaPessoalObserver`, `RewardsVotoPremioObserver` — atribuem pontos automaticamente após eventos de domínio
- **Acessos**: `AcessoObserver` — regista acessos após compra finalizada

### Adapter / Facade
- `TmdbApiClient` + `TmdbService` — isola a API TMDB; controllers e services nunca chamam TMDB diretamente

### Repository
- Um repositório por domínio: `IFilmeRepository`, `IFestivalRepository`, `ISessaoRepository`, `ICompraRepository`, `ICarrinhoRepository`, `IListaPessoalRepository`, `IComunidadeRepository`, `IComentarioRepository`, `IReporteUtilizadorRepository`, etc.

### Seed (orquestrador + passos)
- `DbSeeder` — orquestrador puro que delega a 11 `ISeedStep` em ordem de dependência:
  `UtilizadoresSeeder → FilmesSeeder → FestivaisSeeder → SessoesSeeder → AcessosSeeder → ComprasSeeder → ComunidadesSeeder → ReviewsSeeder → ListasSeeder → PremiosSeeder → RewardsSeeder`

---

## Principais funcionalidades

### Autenticação e Perfil
- Registo com validação de email, telefone, password forte e confirmação
- Login JWT com expiração configurável
- Perfil público/privado com upload de foto (jpg, jpeg, png, webp; máx. 2 MB; validação de magic bytes)
- Bandeira por país, localidade, bio, géneros favoritos
- Área pessoal: histórico de compras, acessos ativos, listas, rewards, atividade recente

### Catálogo e Filmes
- Pesquisa local prioritária; TMDB aparece apenas quando não há resultados internos relevantes
- Ordenação por título, classificação, popularidade, data, visualizações, festival
- Detalhe com trailer TMDB/YouTube, realizador, atores, géneros, reviews internas, prémios, sessões e acessos
- Importação de filmes TMDB restrita ao admin

### Festivais e Sessões
- Festivais com datas de início/fim, filmes associados, passes, votação e resultados publicados
- Sessões com estado (futura, em curso, terminada), tipo (normal, chat ao vivo), acesso necessário
- Chat ao vivo em tempo real via SignalR durante sessões ativas

### Compras e Acessos
- Carrinho com até 99 itens
- Tipos de acesso: Bilhete de Sessão, Passe Diário, Passe Completo, Aluguer Digital 48h
- Pagamentos simulados: Cartão de Crédito (aprovação imediata) e Referência Multibanco (pendente com expiração configurável)
- Expiração automática de pagamentos Multibanco por background service

### Rewards
- Sistema de pontos atribuídos por eventos: avaliação, comentário, visualização, lista pessoal, voto em prémio
- Histórico de transações visível na área do utilizador

### Comunidades
- Criação por qualquer utilizador registado
- Código de convite para acesso restrito
- Comentários com moderação pelo proprietário (ocultar/remover)
- Reporte de utilizadores com gestão pelo admin

### Prémios
- Prémios por festival criados pelo admin
- Votação do público (um voto por utilizador por prémio)
- Publicação de vencedores automática por background service quando o festival termina

### Listas Pessoais
- Listas predefinidas: Quero ver, Vistos, Favoritos (não apagáveis)
- Listas personalizadas com nome único por utilizador (3–50 caracteres)
- Sem duplicados de filme por lista

### Admin
- Dashboard em `/admin`
- Gestão de filmes, festivais, sessões, prémios e publicação de vencedores
- Moderação de reportes de utilizadores
- Importação de filmes via TMDB

---

## Estrutura de pastas

```
OnlineCinemaFestival.Api/
├── Autorizacao/              Constantes de papéis e políticas JWT
├── Configuracao/             Options (Jwt, Cors, Acessos, Pagamentos, Tmdb, YouTube)
├── Controllers/              Controllers finos por domínio
├── Data/
│   ├── AppDbContext.cs
│   ├── AppDbContextFactory.cs   (design-time; lê user-secrets)
│   ├── Configurations/          EF Core fluent configurations
│   └── Seed/                    DbSeeder + 11 seeders por domínio
├── DTOs/                     Data transfer objects de request/response
├── Excecoes/                 Exceções de domínio personalizadas
├── Extensions/               ServiceCollection extensions (registos DI)
├── Hubs/                     SignalR hub do chat de sessão
├── Mappers/                  Mapeamento Model → DTO
├── Middleware/               Error handler global
├── Migrations/               1 migration limpa: FinalSchemaCinemaFestival
├── Models/                   Entidades de domínio
├── Repositories/             Interfaces e implementações EF Core
└── Services/                 Serviços de negócio, strategies, factories, observers

OnlineCinemaFestival.Client/
├── Components/               Componentes Blazor reutilizáveis
├── Configuracao/             Constantes (papéis, métodos de pagamento)
├── Extensions/               ServiceCollection extensions por domínio
├── Layout/                   Layout principal e navbar
├── Models/                   DTOs do lado cliente
├── Pages/                    Páginas Blazor por funcionalidade
├── Services/                 Clientes HTTP por domínio
└── wwwroot/                  Ficheiros estáticos, CSS, imagens, appsettings.json

OnlineCinemaFestival.Tests/
├── Acessos/                  PoliticasAcessoTests, AcessoCompraServiceTests
├── Admin/                    ReporteUtilizadorTests
├── Compras/                  CarrinhoServiceTests, CheckoutValidacaoTests,
│                             FinalizacaoCompraTests, CompraHistoricoTests
├── Comunidades/              ModeracaoComentarioTests
├── Listas/                   ListaSemDuplicadosTests
├── Pagamentos/               PagamentoSimuladoTests, MultibancoExpiracaoTests
├── Perfis/                   PerfilPublicoPrivadoTests
├── Premios/                  VotacaoTests, VencedorAutomaticoTests
├── Recomendacoes/            RecomendacaoTests, CatalogoTests
├── Rewards/                  RewardsPontuacaoTests
├── Upload/                   ImagemValidacaoTests
├── Visualizacoes/            VisualizacaoFluxoTests, ChatTemporalTests
└── Support/
    ├── Builders/             13 fluent builders (Utilizador, Filme, Sessao, Compra…)
    ├── Fakes/                Repositórios falsos por domínio
    ├── Assertions/           Extension methods de asserção (Pagamento, Compra)
    ├── FakeTimeProvider.cs
    └── OpcoesTeste.cs
```

---

## Configuração e arranque

### Pré-requisitos

- **.NET 10 SDK** (≥ 10.0.100) — confirmar com `dotnet --version`
- Ferramenta EF Core: `dotnet tool install --global dotnet-ef`

### 1. Clonar, restaurar e compilar

```bash
git clone <url>
cd ProjetoCinema
dotnet restore
dotnet build OnlineCinemaFestival.slnx
```

### 2. Configurar secrets locais da API

```bash
cd OnlineCinemaFestival.Api

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=festival.db"
dotnet user-secrets set "Jwt:Key" "chave-local-com-pelo-menos-32-caracteres-aleatorios"
dotnet user-secrets set "Jwt:Issuer" "OnlineCinemaFestival"
dotnet user-secrets set "Jwt:Audience" "OnlineCinemaFestivalClient"
dotnet user-secrets set "Cors:AllowedOrigins:0" "http://localhost:5174"
dotnet user-secrets set "Seed:AdminEmail" "admin@festival.pt"
dotnet user-secrets set "Seed:AdminPassword" "Admin123!"
dotnet user-secrets set "Seed:UtilizadorPassword" "User123!"
dotnet user-secrets set "Tmdb:Token" "bearer-token-do-tmdb"
```

> `Tmdb:Token` pode ser omitido se não se pretender usar integração TMDB. Sem `Jwt:Key` ou `ConnectionStrings:DefaultConnection` a API falha no arranque.

> **`appsettings.Development.json` não é versionado.** Todas as configurações sensíveis
> (connection string, JWT key, TMDB token, admin credentials) ficam em `dotnet user-secrets`
> ou num `appsettings.Development.json` local que está no `.gitignore`.

### 3. Configurar o Client

O Client lê a URL da API em `OnlineCinemaFestival.Client/wwwroot/appsettings.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5152/"
  }
}
```

Ajustar a porta se necessário.

### 4. Criar a base de dados e correr o seeder

```bash
cd ..
dotnet ef database update \
  --project OnlineCinemaFestival.Api \
  --startup-project OnlineCinemaFestival.Api
```

O seeder corre automaticamente no primeiro arranque da API (via `MigrateAsync` + `SeedAsync`).

### 5. Arrancar os projetos

Terminal 1 — API:
```bash
dotnet run --project OnlineCinemaFestival.Api
```

Terminal 2 — Client:
```bash
dotnet run --project OnlineCinemaFestival.Client
```

Abrir o browser em `http://localhost:5174`.

---

## Migrations e base de dados

O projeto tem **uma única migration limpa**: `FinalSchemaCinemaFestival`.

### Criar migration nova (após alterar modelos)

```bash
dotnet ef migrations add NomeDaMigration \
  --project OnlineCinemaFestival.Api \
  --startup-project OnlineCinemaFestival.Api
```

### Aplicar migrations

```bash
dotnet ef database update \
  --project OnlineCinemaFestival.Api \
  --startup-project OnlineCinemaFestival.Api
```

### Reset completo da base de dados

```bash
# Apagar a BD local
rm OnlineCinemaFestival.Api/festival.db

# Recriar (migration + seeder correm no primeiro arranque)
dotnet run --project OnlineCinemaFestival.Api
```

> `*.db`, `*.db-shm`, `*.db-wal` estão no `.gitignore` e nunca devem ser commitados.

---

## Testes

### Como correr

```bash
# Compilar + correr todos os testes (sem output de build)
dotnet test OnlineCinemaFestival.Tests

# Com output detalhado
dotnet test OnlineCinemaFestival.Tests --logger "console;verbosity=normal"

# Sequência completa recomendada antes de commit
dotnet clean
dotnet restore
dotnet build OnlineCinemaFestival.slnx
dotnet test OnlineCinemaFestival.Tests
```

**110 testes unitários** organizados por domínio, sem dependências de BD real ou TMDB.

| Pasta | Testes | O que cobre |
|-------|--------|-------------|
| `Acessos/` | 8 | Políticas de acesso por tipo (`PermiteVisualizacao`, `RelacionaComContexto`); criação de acessos pós-compra |
| `Admin/` | 4 | Reporte de utilizadores; permissões de moderação admin |
| `Compras/` | 18 | Carrinho; validação de checkout; finalização de compra; histórico |
| `Comunidades/` | 3 | Moderação de comentários; permissões de dono de comunidade |
| `Integracao/` | 8 | `TmdbService` (Adapter) com cliente HTTP falso — mapping, cache, resiliência a falhas |
| `Listas/` | 10 | Duplicados de filme; duplicados de nome; listas predefinidas não apagáveis |
| `Pagamentos/` | 7 | Pagamento simulado (Cartão/Multibanco); expiração de Multibanco |
| `Perfis/` | 4 | Perfil público/privado; visibilidade entre utilizadores |
| `Prémios/` | 6 | Votação única por utilizador; publicação automática de vencedor |
| `Recomendações/` | 7 | Recomendaç��o por género/avaliação/popularidade/prémios; catálogo com paginação |
| `Rewards/` | 3 | Pontuação por evento (avaliação, comentário, lista) |
| `Social/` | 7 | Observers de Rewards — `ComentarioObserver`, `AvaliacaoObserver`, `VisualizacaoObserver`, `VotoPremioObserver`; idempotência |
| `Upload/` | 9 | Extensão inválida; magic bytes errados; tamanho excedido; tipos aceites |
| `Visualizações/` | 8 | Fluxo completo de visualização (player) com acesso válido/inválido; chat temporal |

**Cobertura por categoria de requisito:**

| Requisito | Testes que cobrem |
|-----------|-------------------|
| Acessos / player | `Acessos/`, `Visualizacoes/VisualizacaoFluxoTests` |
| Checkout | `Compras/CheckoutValidacaoTests`, `Compras/FinalizacaoCompraTests` |
| Permissões admin | `Admin/ReporteUtilizadorTests` |
| Reports / moderação | `Admin/ReporteUtilizadorTests`, `Comunidades/ModeracaoComentarioTests` |
| Rewards | `Rewards/RewardsPontuacaoTests`, `Social/RewardsObserverTests` |
| TMDB fake/mock | `Integracao/TmdbServiceAdapterTests` |

**Padrões usados nos testes:**
- `FakeTimeProvider` — controlo determinístico de datas
- 13 fluent builders (`UtilizadorBuilder`, `FilmeBuilder`, `CompraBuilder`, etc.)
- Repositórios falsos em memória por domínio
- Extension methods de asserção (`DeveEstarPendente()`, `DeveEstarExpirado()`, etc.)

---

## Imagens, logos e uploads

O frontend usa estes assets estáticos (fallback visual se não existirem):

```
OnlineCinemaFestival.Client/wwwroot/images/brand/sky-cinema-logo.svg
OnlineCinemaFestival.Client/wwwroot/images/brand/sky-cinema-mark.svg
OnlineCinemaFestival.Client/wwwroot/images/brand/tmdb-logo.svg
OnlineCinemaFestival.Client/wwwroot/images/placeholders/avatar-placeholder.svg
OnlineCinemaFestival.Client/wwwroot/images/placeholders/poster-placeholder.svg
OnlineCinemaFestival.Client/wwwroot/images/placeholders/festival-placeholder.svg
OnlineCinemaFestival.Client/wwwroot/images/placeholders/community-placeholder.svg
OnlineCinemaFestival.Client/wwwroot/images/illustrations/empty-state-reel.svg
```

Uploads gerados em runtime (ignorados pelo Git):

```
OnlineCinemaFestival.Api/wwwroot/uploads/perfis/
OnlineCinemaFestival.Api/wwwroot/uploads/comunidades/
```

**Regras de upload:** formatos aceites — `jpg`, `jpeg`, `png`, `webp`; tamanho máximo — 2 MB; validação de magic bytes para garantir integridade; nome gerado com `Guid` para evitar colisões.

---

## Credenciais demo

Criadas pelo `DbSeeder` com os valores configurados nos user-secrets:

| Conta | Email | Password |
|-------|-------|----------|
| Administrador | `admin@festival.pt` | `Admin123!` |
| Utilizadores demo | `utilizador1@demo.pt` … | `User123!` |

---

## Fluxo de demonstração

1. **Login como admin** → `/login` com `admin@festival.pt` / `Admin123!`
2. **Importar filmes TMDB** → `/admin` → separador Filmes → Importar TMDB
3. **Criar festival** → `/admin` → Festivais → Novo festival com datas futuras
4. **Associar filmes e criar sessões** → dentro do festival criado
5. **Criar prémios** → `/admin` → Prémios → adicionar categorias ao festival
6. **Login como utilizador** → registo ou utilizador seed
7. **Explorar catálogo** → `/catalogo` → filtrar, ordenar, abrir detalhe de filme
8. **Comprar acesso** → carrinho → checkout → pagar com Cartão ou Multibanco
9. **Assistir sessão** → `/sessoes/{id}` → Entrar → player com chat se aplicável
10. **Avaliar e comentar** → detalhe do filme → submeter review (requer visualização)
11. **Votar em prémio** → página do festival → separador Prémios
12. **Ver rewards** → `/perfil` → separador Rewards
13. **Criar comunidade** → `/comunidades` → Nova comunidade; convidar membros
14. **Admin modera reporte** → `/admin` → Reportes → aceitar ou rejeitar

---

## Limitações conhecidas

- **Pagamentos totalmente simulados** — sem integração real com serviços de pagamento.
- **TMDB opcional** — se `Tmdb:Token` não estiver configurado, trailers e importação não funcionam; o resto da app funciona normalmente.
- **SQLite** — adequado para desenvolvimento e demonstração; não recomendado para produção com carga elevada.
- **SignalR em memória** — chat de sessão não escala horizontalmente sem backplane (Redis, Azure SignalR).
- **Upload local** — imagens guardadas em `wwwroot/uploads/`; numa instalação multi-instância seria necessário armazenamento partilhado.
- **Background services simples** — expiração de Multibanco e publicação de prémios correm in-process; num cenário real usariam Hangfire ou Azure Functions.
