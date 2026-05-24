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

Principios aplicados:

- O Blazor WebAssembly comunica com a API apenas via `HttpClient`; nao referencia EF Core, repositories nem modelos internos da API.
- SRP/DIP: regras de negocio ficam em services dependentes de interfaces; controllers recebem HTTP e devolvem respostas.
- OCP: novos tipos de acesso, pagamento ou ordenacao entram por novas strategies/factories registadas em DI.
- Consistencia transacional: a finalizacao de compra corre dentro de transacao explicita via repository/AppDbContext.

## Configuracao Segura

Nao colocar tokens reais, passwords reais, ficheiros `.db`, `secrets.json` ou `appsettings.Development.json` no Git. O `appsettings.json` deve manter placeholders; valores sensiveis devem vir de user-secrets ou variaveis de ambiente.

Configurar secrets locais da API:

```bash
cd OnlineCinemaFestival.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=onlinecinemafestival.db"
dotnet user-secrets set "Jwt:Key" "chave-local-com-pelo-menos-32-caracteres"
dotnet user-secrets set "Jwt:Issuer" "OnlineCinemaFestival"
dotnet user-secrets set "Jwt:Audience" "OnlineCinemaFestivalClient"
dotnet user-secrets set "Cors:AllowedOrigins:0" "http://localhost:5174"
dotnet user-secrets set "Seed:AdminEmail" "admin@festival.pt"
dotnet user-secrets set "Seed:AdminPassword" "Admin123!"
dotnet user-secrets set "Seed:UtilizadorPassword" "User123!"
dotnet user-secrets set "Tmdb:Token" "token-read-access-do-tmdb"
```

`Tmdb:Token` e `YouTube:ApiKey` podem ser omitidos quando nao se pretende usar essas integracoes. Sem `Jwt:Key`, connection string ou CORS valido, a API falha no arranque por configuracao de seguranca.

O Client le a API em `OnlineCinemaFestival.Client/wwwroot/appsettings.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5152/"
  }
}
```

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

Em `Development`, o `DbSeeder` cria/atualiza dados de demo com as credenciais configuradas em user-secrets.

Correr testes:

```bash
dotnet test
```

## Padroes de Desenho Aplicados

| Padrao | Onde esta | Problema que resolve | SOLID | Beneficio | Trade-off |
| --- | --- | --- | --- | --- | --- |
| Repository | `Repositories/*Repository.cs` | Isola EF Core e queries | DIP, SRP | Trocar persistencia ou testar services fica mais simples | Mais interfaces e classes |
| Service Layer | `Services/*Service.cs` | Centraliza regras de negocio fora dos controllers | SRP | Controllers finos e reutilizacao de casos de uso | Services podem crescer se nao forem divididos |
| Strategy | validators de carrinho, pagamentos, catalogo, validacao de acesso | Varia comportamento por tipo | OCP | Novo tipo entra por nova classe e DI | Mais registos em DI |
| Factory | `CompraFactory`, `AcessoAutomaticoFactory`, `AcessoUtilizadorFactory` | Cria objetos complexos de forma consistente | SRP, OCP | Evita construcao espalhada | Exige nomes claros para nao esconder regra |
| Resolver | `PoliticaAcessoResolver`, factories de strategies | Escolhe implementacao correta em runtime | DIP, OCP | Reduz `switch` em services | Falhas de registo aparecem em runtime |
| Template Method | `CarrinhoItemValidatorBase` | Fluxo comum para validar itens do carrinho | SRP | Remove duplicacao entre validators | Base class deve ficar pequena |
| Observer/Eventos | `ICompraObserver`, `IVisualizacaoObserver`, observers de rewards | Efeitos secundarios sem acoplar fluxo principal | DIP | Rewards/acessos evoluem sem mexer no checkout | Ordem/atomicidade precisam de cuidado |
| DTO/Mapper | `DTOs/*`, `Mappers/*Mapper.cs` | Separa contrato publico dos modelos EF | ISP, SRP | Rotas ficam estaveis mesmo com modelo interno | Mais codigo de mapping |

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
