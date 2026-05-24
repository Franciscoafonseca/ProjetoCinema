# Nota tecnica: TMDB e provider SkyShowtime

## Conclusao

A API TMDB permite filtrar filmes por providers de streaming atraves de
`discover/movie`, usando `with_watch_providers` em conjunto com `watch_region`.
Tambem disponibiliza a lista de providers por regiao atraves de
`watch/providers/movie`.

Isto nao e suficiente para prometer um catalogo SkyShowtime fiavel na aplicacao:

- a disponibilidade depende sempre da regiao enviada em `watch_region`;
- o identificador do provider e obtido a partir dos dados atuais da TMDB;
- nomes e disponibilidade de providers podem mudar;
- ausencia de provider ou resultados deve ser tratada como dado indisponivel,
  nao como erro da aplicacao.

## Regra no projeto

O backend pode tentar preferir resultados com provider associado a SkyShowtime
quando a TMDB devolver esse provider para Portugal. Se a TMDB nao devolver
provider ou resultados, o sistema deve cair para filmes populares e continuar
sem crash.

Nao apresentar texto de produto a garantir "catalogo SkyShowtime" enquanto nao
existir uma integracao contratual ou uma fonte controlada pelo projeto.

## Referencias oficiais

- TMDB Discover Movie: `with_watch_providers` deve ser usado com `watch_region`.
- TMDB Watch Providers Movie: devolve providers de filmes por regiao.
