using Autofac;
using LiveCommand.Common.NetMQ;
using LiveFactory.Application.Command;
using Microsoft.Extensions.Configuration;
using NetMQ.Sockets;
using System.Configuration;

namespace LiveFactory.Application
{
    public class ApplicationModule : Module
    {
      
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ApplicationModule).Assembly)
                 .Where(w => w.Name.EndsWith("Service"))
                 .AsImplementedInterfaces()
                 .InstancePerLifetimeScope();

            //  builder.RegisterType<CommandService>().As<ICommandService>().InstancePerRequest();

            // builder.Register(c => new NetMQManage(str)).As<INetMQManage>().SingleInstance();
            builder.RegisterType<PublisherSocket>().As<PublisherSocket>().SingleInstance();
            builder.RegisterType<SendCommandManage>().As<ISendCommand>().SingleInstance();
            //builder.RegisterType<LiveChannelManager>().As<ILiveChannelManager>().SingleInstance();
           
        }
    }
}
