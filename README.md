# Proyecto de Implementación de Arquitectura en la Nube ANV3-229

**Sistema de riego inteligente en la industria de flores**  
Autores: Edwin Rodriguez Barrera, Adriana Moya Aguirre, Yhosi Esteban Dulcey, Julian González  
Instructores: Ing. Nicolas Linares, Ing. Aroldo Arrieta  
Organización: Cymetria – Tecnalia Colombia / Talento Tech Colombia  
Fecha: Octubre 2024

## Descripción del Proyecto

Este proyecto introduce un sistema de riego automatizado para la industria de flores en Colombia. Utilizando una plataforma web en Amazon AWS, el sistema ajusta el riego en función de condiciones ambientales, lo que reduce el uso de agua y aumenta la eficiencia en la producción de flores.

## Justificación

La floricultura es clave para la economía colombiana, generando aproximadamente 200,000 empleos anuales. Este proyecto busca mejorar el manejo del riego y reducir el impacto ambiental del cultivo de flores mediante la implementación de tecnologías avanzadas y prácticas sostenibles.

## Objetivos

### Objetivo General
Desarrollar una aplicación en la nube Amazon AWS que permita automatizar el riego en cultivos de flores, utilizando datos de telemetría y tecnologías IoT.

### Objetivos Específicos
1. Analizar la situación actual del riego en la región de Cundinamarca y proponer mejoras.
2. Diseñar una aplicación web para controlar el riego, integrando software y hardware.
3. Evaluar el impacto del sistema de riego automatizado en la eficiencia y ahorro de agua.

## Arquitectura de la Solución

1. **Interfaz de usuario**: Accesible desde PC, celular o tablet.
2. **Amazon Route 53**: Servicio DNS para conectar usuarios con la solución web.
3. **AWS WAF**: Firewall para proteger contra ataques.
4. **API Gateway**: Administra las llamadas a la API del backend.
5. **Lambda**: Procesa solicitudes web y de IoT.
6. **EC2 Instances**: Servidores virtuales donde se aloja el sitio web.
7. **Elastic Load Balancer**: Distribuye el tráfico entrante entre los servidores.
8. **DynamoDB**: Base de datos noSQL para alojar los datos de riego.
9. **Amazon S3**: Almacenamiento estático y backup.

## Cronograma

El proyecto tiene una duración estimada de 16 días, abarcando desde la revisión inicial hasta el despliegue en producción y entrega final.

| Fecha         | Actividad                                      |
|---------------|-----------------------------------------------|
| 16 octubre    | Revisión de documentación                     |
| 17 octubre    | Análisis del problema                         |
| 18-23 octubre | Definición de arquitectura y desarrollo       |
| 24 octubre    | Configuración de bases de datos y almacenamiento |
| 25 octubre    | Definición de políticas de ciberseguridad     |
| 26-31 octubre | Despliegue, pruebas de usuario, y cierre      |

## Análisis de Riesgos

1. **Mala configuración**: Puede facilitar ataques de ransomware, cryptomining, entre otros.
2. **Violación de datos**: Debido a errores humanos o ataques dirigidos.
3. **Vulnerabilidades del sistema**: Fallos de seguridad explotables.
4. **Gestión de identidades**: Accesos deficientes pueden facilitar robo de datos.
5. **Amenazas internas**: Usuarios malintencionados con acceso a información confidencial.
6. **Robo de cuentas**: Puede permitir el control indebido de la información.
7. **Ataques DDoS**: Interrupción de servicios mediante sobrecarga de solicitudes.

## Costos Estimados

### Personal
- Programador: $12,000,000 COP
- Analista: $12,000,000 COP
- **Total personal**: $24,000,000 COP

### Entorno Cloud
Costos relativos y basados en el uso de servicios como Amazon S3, DynamoDB y Lambda.

## Conclusiones

La implementación de soluciones en la nube permite reducir costos y aumentar la eficiencia en el uso de recursos. Este proyecto destaca por su contribución al ahorro de agua, el control ambiental y la mejora en la calidad de producción de flores en Colombia.

## Insturciones de instalación AWS CDK - Proyecto CDKAppFlores


Instrucciones de Instalación y Configuración de AWS CDK

1. Instalar Node.js y NPM
El AWS CDK requiere Node.js y NPM. Puedes verificar si ya los tienes instalados ejecutando estos comandos en tu terminal:

    node -v
    npm -v

Si no tienes Node.js y NPM, puedes descargarlos desde https://nodejs.org/.

2. Instalar el AWS CDK
Una vez que tengas Node.js y NPM, puedes instalar el AWS CDK con el siguiente comando:

    npm install -g aws-cdk

Esto instalará el comando `cdk`, que usarás para interactuar con el AWS CDK.

3. Configurar Credenciales de AWS
Asegúrate de tener configuradas tus credenciales de AWS en tu entorno. Puedes hacerlo usando la CLI de AWS:

    # Instalar la CLI de AWS si aún no la tienes:
    pip install awscli

    # Configura tus credenciales:
    aws configure

Aquí, se te pedirá ingresar:
- Access Key ID
- Secret Access Key
- Región (por ejemplo, `us-east-1`)
- Formato de salida (puedes usar `json`)

4. Compilar el Proyecto en C#
Antes de desplegar la infraestructura, necesitas compilar tu proyecto C# usando `dotnet`:

    dotnet build src/

Esto generará un archivo `bin` con la versión compilada de tu aplicación CDK.

5. Sintetizar la Infraestructura
La "sintetización" crea un archivo de plantilla de CloudFormation basado en el código de CDK.

    cdk synth

Este comando generará la plantilla de CloudFormation que luego será desplegada.

6. Desplegar la Infraestructura
Para desplegar tu infraestructura en AWS, usa:

    cdk deploy

Este comando enviará la plantilla de CloudFormation a AWS y comenzará a crear los recursos especificados en tu código.

7. Limpiar la Infraestructura (Opcional)
Cuando ya no necesites la infraestructura, puedes eliminarla con:

    cdk destroy

Resumen de Comandos
Aquí tienes un resumen rápido de los comandos que necesitarás:

    # Instalar el AWS CDK
    npm install -g aws-cdk

    # Inicializar un proyecto en C#
    cdk init app --language csharp

    # Compilar el proyecto
    dotnet build src/

    # Sintetizar la infraestructura
    cdk synth

    # Desplegar la infraestructura
    cdk deploy

    # Eliminar la infraestructura
    cdk destroy

Siguiendo estos pasos, deberías poder crear y gestionar tu infraestructura en AWS utilizando el AWS CDK en C#.
## Referencias Bibliográficas
- [AWS Lambda y DynamoDB](https://docs.aws.amazon.com/?nc2=h_ql_doc_do)
- [Tecnología de riego inteligente](https://proain.com/blogs/notas-tecnicas/que-es-la-tecnologia-de-riego-inteligente-y-como-cuida-el-futuro-de-la-agricultura)
