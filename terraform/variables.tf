variable "resource_group_name" {
  type        = string
  description = "Name of the Azure Resource Group"
  default     = "tx-remote-monitoring-rg"
}

variable "location" {
  type        = string
  description = "Azure region to deploy to"
  default     = "EastUS"
}

variable "acr_name" {
  type        = string
  description = "Name for Azure Container Registry (must be globally unique)"
  default     = "txremoteacregistry123"
}

variable "app_service_name" {
  type        = string
  description = "Name for Azure Web App for Containers"
  default     = "tx-robotcontrolservice"
}

variable "app_service_plan_name" {
  type        = string
  description = "Name for App Service Plan"
  default     = "tx-robotcontrolservice-plan"
}