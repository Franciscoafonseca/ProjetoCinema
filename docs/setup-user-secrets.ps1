$Project = "OnlineCinemaFestival.Api"

Write-Host "A configurar User Secrets para $Project..." -ForegroundColor Cyan

dotnet user-secrets init --project $Project

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=festival.db" --project $Project

# JWT local de desenvolvimento. Pode ser diferente em cada PC.
dotnet user-secrets set "Jwt:Key" "chave-local-desenvolvimento-projeto-cinema-2026-com-mais-de-32-caracteres" --project $Project

dotnet user-secrets set "Jwt:Issuer" "OnlineCinemaFestival" --project $Project
dotnet user-secrets set "Jwt:Audience" "OnlineCinemaFestivalClient" --project $Project
dotnet user-secrets set "Jwt:ExpirationMinutes" "120" --project $Project

dotnet user-secrets set "Seed:AdminEmail" "admin@festival.pt" --project $Project
dotnet user-secrets set "Seed:AdminPassword" "Admin123!" --project $Project
dotnet user-secrets set "Seed:UtilizadorPassword" "User123!" --project $Project

dotnet user-secrets set "Tmdb:BaseUrl" "https://api.themoviedb.org/3/" --project $Project

Write-Host ""
Write-Host "Configuração base concluída." -ForegroundColor Green
Write-Host "Se quiseres usar TMDB, corre depois:"
Write-Host 'dotnet user-secrets set "Tmdb:Token" "O_TEU_TOKEN_TMDB" --project OnlineCinemaFestival.Api'
Write-Host ""
Write-Host "Secrets atuais:"
dotnet user-secrets list --project $Project