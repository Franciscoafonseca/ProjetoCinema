# Online Cinema Festival

Plataforma academica para festivais de cinema online, com backend ASP.NET Core Web API, frontend Blazor WebAssembly, EF Core e SQLite.

## Funcionalidades

- Registo, login JWT, perfis privados/publicos e roles de utilizador/admin.
- Catalogo de filmes com dados TMDB, pesquisa/importacao TMDB e detalhe de filme.
- Festivais, associacao de filmes a festivais e sessoes digitais.
- Acessos: bilhete de sessao, passe diario, passe completo e aluguer digital.
- Carrinho, checkout com pagamento simulado e historico de compras.
- Validacao de acesso antes do player.
- Chat ao vivo por sessao com SignalR, historico persistido, nome real do utilizador, validacao anti-spam simples e remocao por moderacao admin.
- Comentarios, avaliacoes, listas pessoais, comunidades, rewards e premios do publico.
- Seed de desenvolvimento com admin, utilizadores, 20 filmes TMDB, festivais, sessoes, acessos e compras de teste.

## Estrutura

```text
OnlineCinemaFestival.Api/      Backend ASP.NET Core Web API
OnlineCinemaFestival.Client/   Frontend Blazor WebAssembly
OnlineCinemaFestival.slnx      Solution
```

Fluxo esperado no backend:

```text
Controller -> Service -> Repository -> AppDbContext
```

Controllers devem ficar magros, services concentram regras de negocio e repositories centralizam o acesso a dados.

## Credenciais de Teste

- Admin: `admin@festival.pt` / `Admin123!`
- Utilizador: `utilizador1@teste.pt` / `User123!`
- Outros utilizadores seed: `utilizador2@teste.pt` ate `utilizador35@teste.pt` / `User123!`

## Requisitos

- .NET SDK 10
- SQLite
- Ferramenta EF Core:

```bash
dotnet tool install --global dotnet-ef
```

ou:

```bash
dotnet tool update --global dotnet-ef
```

## Migrations e Base de Dados

Na raiz:

```bash
dotnet restore
dotnet build
```

Aplicar migrations:

```bash
cd OnlineCinemaFestival.Api
dotnet ef database update
```

Em ambiente `Development`, o `DbSeeder` corre no arranque da API e cria/atualiza dados de demo.

Se forem geradas novas alteracoes ao modelo, criar uma migration:

```bash
cd OnlineCinemaFestival.Api
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

## Executar

API:

```bash
cd OnlineCinemaFestival.Api
dotnet run
```

Frontend:

```bash
cd OnlineCinemaFestival.Client
dotnet run
```

Confirma o URL da API em `OnlineCinemaFestival.Client/Program.cs`. O player de teste do chat usa a API em `http://localhost:5152` quando aberto a partir da pagina de visualizacao.

## Demo Rapida

1. Arrancar a API em `Development` para aplicar migrations e seed.
2. Arrancar o frontend.
3. Entrar como `utilizador1@teste.pt`.
4. Abrir `Catalogo` para ver os 20 filmes TMDB seed/importados.
5. Abrir `Sessoes`, comprar um bilhete ou usar o acesso seed.
6. Abrir o player da sessao com chat ao vivo.
7. Enviar mensagens; o chat mostra o nome real e carrega historico persistido.
8. Entrar como admin e abrir o player com `?admin=1` para remover mensagens do chat.
9. Testar checkout em `Carrinho`/`Finalizacao da compra` e consultar historico em `Minhas compras`.

## Comandos Uteis

```bash
dotnet clean
dotnet restore
dotnet build
dotnet build OnlineCinemaFestival.Api/OnlineCinemaFestival.Api.csproj
dotnet build OnlineCinemaFestival.Client/OnlineCinemaFestival.Client.csproj
```

## Dados Locais Ignorados

Nao enviar para Git:

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
