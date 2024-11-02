using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.DynamoDB;

using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace CDK
{
    public class ProyectoStack : Stack
    {
        // Constructor de la clase ProyectoStack
        internal ProyectoStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Definir una política de acceso para DynamoDB
            var dynamoPolicy = new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "dynamodb:*" }, // Permitir todas las acciones en DynamoDB
                Resources = new[] { "*" } // Opcional: Cambia esto a los ARNs específicos de tus tablas para restringir el acceso
            });

            // Crear un rol para instancias EC2 que usarán DynamoDB
            var ec2Role = new Role(this, "EC2DynamoDBRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com") // Define que este rol será asumido por instancias EC2
            });

            // Asignar la política de acceso de DynamoDB al rol EC2
            ec2Role.AddToPolicy(dynamoPolicy);

            // Crear una VPC (Virtual Private Cloud) para los recursos
            var vpc = new Vpc(this, "ProyectoVPC", new VpcProps
            {
                MaxAzs = 2 // Número máximo de zonas de disponibilidad
            });

            // Crear un bucket de S3 para almacenamiento estático y copias de seguridad
            var s3Bucket = new Bucket(this, "ProyectoBucket", new BucketProps
            {
                Versioned = true // Activar versionado para el bucket
            });

            // Crear una tabla DynamoDB para almacenar datos de la aplicación
            var mainTable = new Table(this, "AppDataFlores", new TableProps
            {
                PartitionKey = new Attribute { Name = "Id", Type = AttributeType.STRING }, // Clave principal
                BillingMode = Amazon.CDK.AWS.DynamoDB.BillingMode.PAY_PER_REQUEST // Modo de facturación bajo demanda
            });

            // Crear otra tabla DynamoDB para almacenar respuestas
            var replyTable = new Table(this, "AppReplyData", new TableProps
            {
                PartitionKey = new Attribute { Name = "ReplyId", Type = AttributeType.STRING }, // Clave principal
                BillingMode = Amazon.CDK.AWS.DynamoDB.BillingMode.PAY_PER_REQUEST // Modo de facturación bajo demanda
            });

            // Crear una función Lambda
            var lambdaFunction = new Function(this, "ProyectoLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6, // Versión de runtime de Lambda
                Handler = "index.handler", // Nombre del método manejador
                Code = Code.FromAsset("CreacionAutomaticaData") // Ruta al código fuente
            });

            // Crear un API Gateway que exponga la función Lambda como una API REST
            var api = new LambdaRestApi(this, "ProyectoApi", new LambdaRestApiProps
            {
                Handler = lambdaFunction // Asociar la función Lambda al API Gateway
            });

            // Crear un Load Balancer (Balanceador de Carga)
            var lb = new ApplicationLoadBalancer(this, "ProyectoLB", new ApplicationLoadBalancerProps
            {
                Vpc = vpc, // Asociar el Load Balancer a la VPC
                InternetFacing = true // Hacer que el Load Balancer sea accesible desde Internet
            });

            // Añadir un listener para el Load Balancer
            var listener = lb.AddListener("Listener", new BaseApplicationListenerProps
            {
                Port = 80 // Puerto en el que escucha el Load Balancer
            });

            // Crear un grupo de seguridad para las instancias EC2
            var securityGroup = new SecurityGroup(this, "InstanceSecurityGroup", new SecurityGroupProps
            {
                Vpc = vpc, // Asociar el grupo de seguridad a la VPC
                AllowAllOutbound = true // Permitir tráfico saliente
            });

            // Permitir tráfico entrante en el puerto 22 (SSH) para acceso remoto
            securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(22), "Permitir acceso SSH");




            // Crear una imagen AMI (Amazon Machine Image) para instancias Windows
            var machineImage = new WindowsImage(WindowsVersion.WINDOWS_SERVER_2003_R2_SP2_LANGUAGE_PACKS_32BIT_BASE, new WindowsImageProps
            {
                 UserData = UserData.Custom($@"
                    <powershell>
                        # Actualizar el sistema
                        Install-WindowsFeature -name Web-Server -IncludeManagementTools

                        # Configurar el firewall para permitir tráfico en el puerto 80
                        New-NetFirewallRule -DisplayName 'Allow HTTP' -Direction Inbound -Protocol TCP -Action Allow -LocalPort 80

                        # Iniciar el servicio de IIS
                        Start-Service W3SVC
                    </powershell>
                ")
            }
           );

            // Crear un grupo de Auto Scaling para las instancias EC2
           var autoScalingGroup = new AutoScalingGroup(this, "MyAutoScalingGroup", new AutoScalingGroupProps
            {
                Vpc = vpc, // Asociar el Auto Scaling Group a la VPC
                MachineImage = machineImage, // Especificar la imagen de la máquina
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE2, InstanceSize.MICRO), // Tipo de instancia
                MinCapacity = 1, // Capacidad mínima
                MaxCapacity = 2, // Capacidad máxima
                SecurityGroup = securityGroup, // Asignar el grupo de seguridad aquí
                Role = ec2Role //Rol para poder consumir Dynamo
                

           });
            // Asignar el UserData al grupo de Auto Scaling

            // Añadir una política de escalado basado en la utilización de CPU
            var scalingPolicy = new TargetTrackingScalingPolicy(this, "CpuScalingPolicy", new TargetTrackingScalingPolicyProps
            {
                AutoScalingGroup = autoScalingGroup, // Asociar la política al Auto Scaling Group
                TargetValue = 50, // Objetivo de utilización de CPU del 50%
                PredefinedMetric = PredefinedMetric.ASG_AVERAGE_CPU_UTILIZATION, // Métrica predefinida para la CPU
            });

            // Crear una política de acceso para IoT Core
            var iotPolicy = new Policy(this, "IoTPolicy", new PolicyProps
            {
                PolicyName = "ProyectoIoTPolicy", // Nombre de la política
                Statements = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Actions = new[] { "iot:*" }, // Permitir todas las acciones de IoT
                        Resources = new[] { "*" } // Permitir acceso a todos los recursos de IoT
                    })
                }
            });

            // Crear un rol para IoT
            var iotRole = new Role(this, "IoTRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("iot.amazonaws.com") // Este rol será asumido por IoT
            });

            // Adjuntar la política de IoT al rol de IoT
            iotRole.AttachInlinePolicy(iotPolicy);
        }
    }
}
