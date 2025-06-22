provider "azurerm" {
  features {}
}

# 1. Resource Group
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

# 2. Azure Container Registry
resource "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true
}

# 3. App Service Plan (Linux)
resource "azurerm_app_service_plan" "asp" {
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  sku {
    tier = "Basic"
    size = "B1"
  }

  reserved = true  # For Linux plan
}

# 4. Web App for Containers
resource "azurerm_app_service" "webapp" {
  name                = var.app_service_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  app_service_plan_id = azurerm_app_service_plan.asp.id

  site_config {
    linux_fx_version = "DOCKER|${azurerm_container_registry.acr.login_server}/robotcontrolservice:latest"
  }

# app_settings = {
#   "WEBSITE_RUN_FROM_PACKAGE" = "1"
#   "MONGO__ConnectionString"  = azurerm_cosmosdb_account.cosmos.mongo_connection_strings[0]
#   "MONGO__DatabaseName"      = "RemoteMonitoringDb"
#   "JwtSettings__SecretKey"   = "YourSuperSecretKey_ChangeMeInProduction"
#   "JwtSettings__Issuer"      = "TelexistenceInc"
#   "JwtSettings__Audience"    = "TelexistenceClients"
#   "JwtSettings__ExpiryMinutes" = "60"
# }

  identity {
    type = "SystemAssigned"
  }
}

output "acr_login_server" {
  value = azurerm_container_registry.acr.login_server
}

output "app_service_default_site_hostname" {
  value = azurerm_app_service.webapp.default_site_hostname
}
