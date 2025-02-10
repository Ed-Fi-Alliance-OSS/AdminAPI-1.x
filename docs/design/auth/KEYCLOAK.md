# Keycloak Configuration

> [!IMPORTANT]
> Detailed notes to be filled out, replacing these bullet points.
>
> * Should make sure we can configure Keycloak once for use both by Admin API
>   and by Admin Console.
> * Review the configuration script used by the  Data Management Service. The
>   configuration script approach is not required; Keycloak may have a better
>   way of handling this. We just need to get both projects to use the same
>   settings. Maybe create a Docker container for Ed-Fi Keycloak with shared
>   settings already in place.
> * Need to describe how to create the role for the worker processes, which
>   allows them to access client credentials via the `/adminconsole/instances`
>   endpoint.
