# Online Cinema Festival

Plataforma academica para festivais de cinema online, com ASP.NET Core Web API, Blazor WebAssembly, EF Core e SQLite.

## Funcionalidades Reais

- Registo e login JWT com validacoes claras para email, telefone, palavra-passe forte, confirmacao e campos obrigatorios.
- Perfil privado/publico com upload de foto por ficheiro, bandeira por pais, localidade, bio, listas e atividade recente.
- Home com seccoes Mais populares, Mais vistos, Em destaque e Catalogo.
- Catalogo interno com pesquisa local primeiro; TMDB so aparece quando nao ha resultados internos relevantes.
- Detalhe de filme com trailer TMDB/YouTube, realizador, atores, generos, reviews internas, premios vencidos, acessos e sessoes.
- Detalhe TMDB separado: apenas Admin pode importar/adicionar ao catalogo.
- Festivais com filmes associados, sessoes, passes, votacao do publico e resultados publicados.
- Sessoes em `/sessoes/{id}` com festival, filme, inicio/fim, estado, acesso necessario e acoes Comprar/Entrar.
- Carrinho e checkout com bilhete de sessao, passe diario, passe completo e aluguer digital de 48h.
- Player para filme/sessao com validacao de acesso e registo de visualizacao.
- Reviews de 10 estrelas com texto validado e bloqueio ate existir visualizacao valida.
- Comunidades com pagina propria, membros, privacidade, comentarios, autor, perfil publico/privado e filme associado opcional.
- Admin unico em `/admin` para gerir importacoes TMDB, filmes, festivais, sessoes, premios e publicacao de vencedores.

## Arquitetura

```text
OnlineCinemaFestival.Api/      Backend ASP.NET Core Web API
OnlineCinemaFestival.Client/   Frontend Blazor WebAssembly
OnlineCinemaFestival.slnx      Solution
```

Fluxo esperado no backend:

```text
Controller -> Service -> Repository -> AppDbContext
```

Controllers ficam finos, services concentram regras de negocio e repositories centralizam acesso a dados.

Principios e padroes aplicados:

- Caminho B: o Blazor WebAssembly comunica com a API apenas via `HttpClient`; nao referencia EF Core, repositories nem modelos internos da API.
- SRP/DIP: regras de negocio ficam em services dependentes de interfaces; controllers so recebem HTTP e devolvem respostas.
- Repository: queries EF Core ficam em repositories, com `AsSplitQuery` nas leituras com multiplos `Include`.
- Strategy/OCP: validacao de tipos de acesso, ordenacao de catalogo e pagamentos usam strategies extensiveis.
- Factory: acessos automaticos de catalogo sao criados por `IAcessoAutomaticoFactory`.
- Adapter/Facade: chamadas TMDB passam por `ITmdbApiClient` e `ITmdbService`.
- Observer: compras, visualizacoes e avaliacoes notificam observers para rewards/acessos sem acoplar os fluxos principais.

## Configuracao

Preencher `OnlineCinemaFestival.Api/appsettings.json` antes de arrancar a API:

```json
{
  "Jwt": {
    "Key": "chave-local-com-pelo-menos-32-caracteres",
    "Issuer": "OnlineCinemaFestival",
    "Audience": "OnlineCinemaFestivalClient"
  },
  "Tmdb": {
    "Token": "token-read-access-do-tmdb"
  }
}
```

Sem `Jwt:Key`, a API falha no arranque por configuracao de seguranca.

## Credenciais de Demo

- Admin: `admin@festival.pt` / `Admin123!`
- Utilizador: `utilizador1@teste.pt` / `User123!`
- Outros utilizadores seed: `utilizador2@teste.pt` ate `utilizador35@teste.pt` / `User123!`

## Executar

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

Arrancar API:

```bash
dotnet run --project OnlineCinemaFestival.Api
```

Arrancar frontend:

```bash
dotnet run --project OnlineCinemaFestival.Client
```

Confirmar o URL da API em `OnlineCinemaFestival.Client/Program.cs`. Em `Development`, o `DbSeeder` cria/atualiza dados de demo.

## Fluxo de Demo

1. Entrar como `admin@festival.pt`.
2. Abrir `/admin`, pesquisar no TMDB e adicionar um filme ao catalogo.
3. Criar festival, associar filme ao festival e criar uma sessao.
4. Criar premio de festival, abrir votacao, fechar e publicar resultados.
5. Confirmar o vencedor no detalhe do festival e no detalhe do filme.
6. Entrar como `utilizador1@teste.pt`.
7. Pesquisar no Catalogo; abrir filme interno ou detalhe TMDB conforme o resultado.
8. Comprar acesso no detalhe do filme/sessao, finalizar checkout e abrir o player.
9. Depois de uma visualizacao valida, criar review de 10 estrelas e comentario.
10. Abrir Comunidades, entrar/criar comunidade, comentar e associar opcionalmente um filme.
11. Editar Perfil, enviar foto por ficheiro e confirmar bandeira do pais.

## Comandos Uteis

```bash
dotnet clean
dotnet restore
dotnet build OnlineCinemaFestival.slnx
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

# Online Cinema Festival

**Autores do Projeto:**
* Francisco Afonseca - 2120622
* Francisco Palmeira - 2109923
* Afonso Santos - 2141823
* Bernardo Pestana - 2107023