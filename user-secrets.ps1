#dotnet user-secrets -p Neurons.api init
#dotnet user-secrets -p Neurons.api set "STG_URL" "TenantUrl"
#dotnet user-secrets -p Neurons.api set "STG_API_KEY" "API key"
#dotnet user-secrets -p Neurons.api set "UAT_URL" "TenantUrl"
#dotnet user-secrets -p Neurons.api set "UAT_API_KEY" "API key"
dotnet user-secrets -p Neurons.api list 