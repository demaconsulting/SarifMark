package OtsDependencies {
    private import SoftwareItemMetadata::*;

    doc /* Off-the-shelf (OTS) production dependencies used by SarifMark. */

    part def TestResultsPackage {
        doc /* OTS: DemaConsulting.TestResults. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/test-results.md");
            verificationFiles = ("docs/verification/ots/test-results.md");
            requirementFiles = ("docs/reqstream/ots/test-results.yaml");
        }
    }

    part def SysML2ToolsTool {
        doc /* OTS: DemaConsulting.SysML2Tools. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/sysml2tools.md");
            verificationFiles = ("docs/verification/ots/sysml2tools.md");
            requirementFiles = ("docs/reqstream/ots/sysml2tools.yaml");
        }
    }

    part def FileSystemGlobbingPackage {
        doc /* OTS: Microsoft.Extensions.FileSystemGlobbing. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/filesystem-globbing.md");
            verificationFiles = ("docs/verification/ots/filesystem-globbing.md");
            requirementFiles = ("docs/reqstream/ots/filesystem-globbing.yaml");
        }
    }
}
