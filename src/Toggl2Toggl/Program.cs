using AdoIntegration;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;

namespace Toggl2Toggl
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var showCommand = GetShowCommand();
            var syncCommand = GetSyncCommand();

            rootCommand.AddCommand(showCommand);
            rootCommand.AddCommand(syncCommand);

            return rootCommand.InvokeAsync(args).Result;
        }

        private static Command GetSyncCommand()
        {
            var command = new Command("sync");
            command.Description = "Calculates all time entries from the provided toggl workspace and performs a synchronization to the other workspace";

            command.AddOption(new Option(new string[] { "--toggle-api-key", "--toggle" }, "Api key from your Toggl Account", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--ado-personal-access-token", "--ado" }, "Personal Access Token from your Azure DevOps Account", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--ado-organization-url", "--url" }, "URL to your organization in Azure DevOps", new Argument<Uri>()));
            command.AddOption(new Option(new string[] { "--from-date", "--from" }, "Date from which to track your time", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--to-date", "--to" }, "Date to which to track your time", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--toggle-from-workspace", "--workspace-from" }, "Workspace from which to get the track items", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--toggle-to-workspace", "--workspace-to" }, "Workspace to which to create the new items", new Argument<string>
            {
                Arity = ArgumentArity.ExactlyOne
            }));

            command.Handler = CommandHandler.Create<string, string, Uri, string, string, string, string>((toggle, ado, url, from, to, workspaceFrom, workspaceTo) =>
             {


                 var toggleApiKey = toggle;
                 var adoPersonalAccessToken = ado;
                 var orgUrl = url;

                 var startDate = from;
                 var endDate = to;

                 var toggl2ToggleIntegration = new Toggl2TogglIntegration(toggleApiKey);
                 var adoClient = new AdoIntegrationClient(orgUrl, adoPersonalAccessToken);
                 toggl2ToggleIntegration.AddClientResolver(new MvClientResolverBasedOnAdoClientTag(adoClient, 100));
                 toggl2ToggleIntegration.AddClientResolver(new PhraseTimeEntryBasedResolver<IClient>(new MvClientPhraseMapping(), 50));
                 toggl2ToggleIntegration.AddClientResolver(new DefaultTimeEntryBasedResolver<IClient>(MvClient.MarketVision, 1));


                 //toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoAreadPath(new AdoIntegrationClient(orgUrl, adoPersonalAccessToken), 100));
                 toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoItemType(adoClient, 100));
                 toggl2ToggleIntegration.AddProjectResolver(new PhraseTimeEntryBasedResolver<IProject>(new MvProjectPhraseMapping(), 50));
                 toggl2ToggleIntegration.AddProjectResolver(new DefaultTimeEntryBasedResolver<IProject>(MvProject.NewGenSupport, 1));

                 toggl2ToggleIntegration.AddTagResolver(new MvTagResolveBasedOnAdo(MvTag.Development, adoClient, 50));
                 toggl2ToggleIntegration.AddTagResolver(new PhraseTimeEntryBasedResolver<ITag>(new MvTagPhraseMapping(), 40));

                 toggl2ToggleIntegration.Sync(DateTime.Parse(from, CultureInfo.InvariantCulture), DateTime.Parse(to, CultureInfo.InvariantCulture), workspaceFrom, workspaceTo);

                 Console.ReadKey();
             });

            return command;
        }

        private static Command GetShowCommand()
        {
            var command = new Command("show");
            command.Description = "Shows all calculated entries which could be added to the workspace";

            command.AddOption(new Option(new string[] { "--toggle-api-key", "--toggle" }, "Api key from your Toggl Account", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--ado-personal-access-token", "--ado" }, "Personal Access Token from your Azure DevOps Account", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--ado-organization-url", "--url" }, "URL to your organization in Azure DevOps", new Argument<Uri>()));
            command.AddOption(new Option(new string[] { "--from-date", "--from" }, "Date from which to track your time", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--to-date", "--to" }, "Date to which to track your time", new Argument<string>()));
            command.AddOption(new Option(new string[] { "--toggle-from-workspace", "--workspace-from" }, "Workspace from which to get the track items", new Argument<string>()));

            command.Handler = CommandHandler.Create<string, string, Uri, string, string, string>((toggle, ado, url, from, to, workspaceFrom) =>
            {
                var toggleApiKey = toggle;
                var adoPersonalAccessToken = ado;
                var orgUrl = url;

                var startDate = from;
                var endDate = to;

                var toggl2ToggleIntegration = new Toggl2TogglIntegration(toggleApiKey);
                var adoClient = new AdoIntegrationClient(orgUrl, adoPersonalAccessToken);
                toggl2ToggleIntegration.AddClientResolver(new MvClientResolverBasedOnAdoClientTag(adoClient, 100));
                toggl2ToggleIntegration.AddClientResolver(new PhraseTimeEntryBasedResolver<IClient>(new MvClientPhraseMapping(), 50));
                toggl2ToggleIntegration.AddClientResolver(new DefaultTimeEntryBasedResolver<IClient>(MvClient.MarketVision, 1));


                //toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoAreadPath(new AdoIntegrationClient(orgUrl, adoPersonalAccessToken), 100));
                toggl2ToggleIntegration.AddProjectResolver(new MvProjectResolveBasedOnAdoItemType(adoClient, 100));
                toggl2ToggleIntegration.AddProjectResolver(new PhraseTimeEntryBasedResolver<IProject>(new MvProjectPhraseMapping(), 50));
                toggl2ToggleIntegration.AddProjectResolver(new DefaultTimeEntryBasedResolver<IProject>(MvProject.NewGenSupport, 1));

                toggl2ToggleIntegration.AddTagResolver(new MvTagResolveBasedOnAdo(MvTag.Development, adoClient, 50));
                toggl2ToggleIntegration.AddTagResolver(new PhraseTimeEntryBasedResolver<ITag>(new MvTagPhraseMapping(), 40));

                toggl2ToggleIntegration.Show(DateTime.Parse(from, CultureInfo.InvariantCulture), DateTime.Parse(to, CultureInfo.InvariantCulture), workspaceFrom);
                //toggl2ToggleIntegration.Sync(startDate, endDate, "Elders workspace", "MarketVision's workspace");

                Console.ReadKey();
            });

            return command;
        }
    }
}
