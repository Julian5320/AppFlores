using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.WAFv2;
using Amazon.CDK.AWS.IoT;

namespace InfraestructuraProyecto
{
    public class ProyectoStack : Stack
    {
        internal ProyectoStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // VPC
            var vpc = new Vpc(this, "ProyectoVPC", new VpcProps
            {
                MaxAzs = 2 // Define dos zonas de disponibilidad
            });

            // S3 Bucket for Static Storage and Backup
            var s3Bucket = new Bucket(this, "ProyectoBucket", new BucketProps
            {
                Versioned = true
            });

            // DynamoDB Tables
            var mainTable = new Table(this, "AppDataFlores", new TableProps
            {
                PartitionKey = new Attribute { Name = "Id", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });

            var replyTable = new Table(this, "AppDataFlores", new TableProps
            {
                PartitionKey = new Attribute { Name = "ReplyId", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });

            // Lambda Function
            var lambdaFunction = new Function(this, "ProyectoLambda", new FunctionProps
            {
                Runtime = Runtime.Net8,
                Handler = "index.handler",
                Code = Code.FromAsset("CreacionAutomaticaData")
            });

            // API Gateway
            var api = new LambdaRestApi(this, "ProyectoApi", new LambdaRestApiProps
            {
                Handler = lambdaFunction
            });


            // WAF Web Application Firewall
            var waf = new CfnWebACL(this, "ProyectoWAF", new CfnWebACLProps
            {
                Scope = "REGIONAL",
                DefaultAction = new CfnWebACL.ActionProperty { Allow = new CfnWebACL.AllowActionProperty() },
                VisibilityConfig = new CfnWebACL.VisibilityConfigProperty
                {
                    CloudWatchMetricsEnabled = true,
                    MetricName = "ProyectoWAF",
                    SampledRequestsEnabled = true
                }
            });

            // Load Balancer
            var lb = new ApplicationLoadBalancer(this, "ProyectoLB", new ApplicationLoadBalancerProps
            {
                Vpc = vpc,
                InternetFacing = true
            });

            var listener = lb.AddListener("Listener", new BaseApplicationListenerProps
            {
                Port = 80
            });
            var securityGroup = new SecurityGroup(this, "InstanceSecurityGroup", new SecurityGroupProps
            {
                Vpc = vpc,
                AllowAllOutbound = true // Permitir tráfico saliente
            });

            // Permitir tráfico entrante en el puerto 22 (SSH)
            securityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(22), "Permitir acceso SSH");

            // Auto Scaling Group with EC2 instances
            var asg = new AutoScalingGroup(this, "ProyectoASG", new AutoScalingGroupProps
            {
                Vpc = vpc,
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE2, InstanceSize.MICRO),
                MachineImage = MachineImage.LatestAmazonLinux(),
                MinCapacity = 1,
                MaxCapacity = 2
                Role = ec2Role // Asociar el rol IAM con permisos de DynamoDB
                SecurityGroup = securityGroup // Asigna el grupo de seguridad


            });

            listener.AddTargets("AppFleet", new AddApplicationTargetsProps
            {
                Port = 80,
                Targets = new[] { asg }
            });
            var dynamoPolicy = new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "dynamodb:*" }, // Permisos para acceder a DynamoDB
                Resources = new[] { "*" } // Opcional: reemplaza con los ARNs específicos de tus tablas si quieres restringir el acceso
            });
            var ec2Role = new Role(this, "EC2DynamoDBRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com")
            });

            ec2Role.AddToPolicy(dynamoPolicy);


            // IoT Core for simulated IoT devices
            var iotPolicy = new Policy(this, "IoTPolicy", new PolicyProps
            {
                PolicyName = "ProyectoIoTPolicy",
                Statements = new[]
                {
                    new PolicyStatement(new PolicyStatementProps
                    {
                        Actions = new[] { "iot:*" },
                        Resources = new[] { "*" }
                    })
                }
            });

            var iotRole = new Role(this, "IoTRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("iot.amazonaws.com")
            });

            iotRole.AttachInlinePolicy(iotPolicy);
        }
    }
}
