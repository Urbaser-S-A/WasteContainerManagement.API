{{/*
Expand the name of the chart.
*/}}
{{- define "wcm-api.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "wcm-api.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "wcm-api.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "wcm-api.labels" -}}
helm.sh/chart: {{ include "wcm-api.chart" . }}
{{ include "wcm-api.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- with .Values.commonLabels }}
{{ toYaml . }}
{{- end }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "wcm-api.selectorLabels" -}}
app.kubernetes.io/name: {{ include "wcm-api.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "wcm-api.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "wcm-api.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Return the PostgreSQL hostname
*/}}
{{- define "wcm-api.postgresql.host" -}}
{{- if .Values.postgresql.enabled }}
{{- printf "%s-postgresql" (include "wcm-api.fullname" .) }}
{{- else }}
{{- .Values.externalDatabase.host }}
{{- end }}
{{- end }}

{{/*
Return the PostgreSQL port
*/}}
{{- define "wcm-api.postgresql.port" -}}
{{- if .Values.postgresql.enabled -}}
5432
{{- else -}}
{{- .Values.externalDatabase.port | default 5432 }}
{{- end -}}
{{- end }}

{{/*
Return the PostgreSQL database name
*/}}
{{- define "wcm-api.postgresql.database" -}}
{{- if .Values.postgresql.enabled }}
{{- .Values.postgresql.auth.database }}
{{- else }}
{{- .Values.externalDatabase.database }}
{{- end }}
{{- end }}

{{/*
Return the PostgreSQL username
*/}}
{{- define "wcm-api.postgresql.username" -}}
{{- if .Values.postgresql.enabled }}
{{- .Values.postgresql.auth.username | default "postgres" }}
{{- else }}
{{- .Values.externalDatabase.username }}
{{- end }}
{{- end }}

{{/*
Return the PostgreSQL secret name containing the password
*/}}
{{- define "wcm-api.postgresql.secretName" -}}
{{- if .Values.postgresql.enabled }}
{{- printf "%s-postgresql" (include "wcm-api.fullname" .) }}
{{- else if .Values.externalDatabase.existingSecret }}
{{- .Values.externalDatabase.existingSecret }}
{{- else }}
{{- printf "%s-external-db" (include "wcm-api.fullname" .) }}
{{- end }}
{{- end }}

{{/*
Return the PostgreSQL secret key for the password
*/}}
{{- define "wcm-api.postgresql.secretKey" -}}
{{- if .Values.postgresql.enabled }}
postgres-password
{{- else }}
{{- .Values.externalDatabase.existingSecretPasswordKey | default "password" }}
{{- end }}
{{- end }}

{{/*
Return true if using passwordless auth (workload identity / managed identity).
*/}}
{{- define "wcm-api.postgresql.isPasswordless" -}}
{{- if and (not .Values.postgresql.enabled) (eq .Values.externalDatabase.auth.mode "workloadIdentity") -}}
true
{{- end -}}
{{- end }}

{{/*
Return the PostgreSQL password.
Returns empty string for passwordless auth modes.
When postgresql.enabled and no password is set, looks up the existing secret first
(so upgrades don't rotate the password) and falls back to a random 16-char string.
*/}}
{{- define "wcm-api.postgresql.password" -}}
{{- if include "wcm-api.postgresql.isPasswordless" . -}}
{{- else if .Values.postgresql.enabled -}}
{{- if .Values.postgresql.auth.password -}}
{{- .Values.postgresql.auth.password }}
{{- else -}}
{{- $secretName := printf "%s-postgresql" (include "wcm-api.fullname" .) -}}
{{- $existingSecret := lookup "v1" "Secret" .Release.Namespace $secretName -}}
{{- if and $existingSecret $existingSecret.data (index $existingSecret.data "postgres-password") -}}
{{- index $existingSecret.data "postgres-password" | b64dec }}
{{- else -}}
{{- randAlphaNum 16 }}
{{- end -}}
{{- end -}}
{{- else -}}
{{- .Values.externalDatabase.password }}
{{- end -}}
{{- end }}

{{/*
Return the full PostgreSQL connection string.
For workload identity: token auth is handled in-app via NpgsqlDataSourceBuilder.UsePeriodicPasswordProvider.
*/}}
{{- define "wcm-api.postgresql.connectionString" -}}
Host={{ include "wcm-api.postgresql.host" . }};Port={{ include "wcm-api.postgresql.port" . }};Username={{ include "wcm-api.postgresql.username" . }};Database={{ include "wcm-api.postgresql.database" . }}
{{- if not (include "wcm-api.postgresql.isPasswordless" .) -}}
;Password={{ include "wcm-api.postgresql.password" . }}
{{- end -}}
{{- if not .Values.postgresql.enabled }}
{{- with .Values.externalDatabase.sslMode }};SSL Mode={{ . }}{{- end }}
{{- end }}
{{- end }}

{{/*
Return the image reference (supports digest pinning).
Digest takes precedence over tag when both are set.
*/}}
{{- define "wcm-api.image" -}}
{{- $repo := .Values.image.repository }}
{{- if .Values.image.registry }}
{{- $repo = printf "%s/%s" .Values.image.registry .Values.image.repository }}
{{- end }}
{{- if .Values.image.digest }}
{{- printf "%s@%s" $repo .Values.image.digest }}
{{- else }}
{{- printf "%s:%s" $repo (.Values.image.tag | default .Chart.AppVersion) }}
{{- end }}
{{- end }}

{{/*
Return the PostgreSQL image reference (supports digest pinning).
*/}}
{{- define "wcm-api.postgresql.image" -}}
{{- $repo := .Values.postgresql.image.repository }}
{{- if .Values.postgresql.image.digest }}
{{- printf "%s@%s" $repo .Values.postgresql.image.digest }}
{{- else }}
{{- printf "%s:%s" $repo .Values.postgresql.image.tag }}
{{- end }}
{{- end }}
