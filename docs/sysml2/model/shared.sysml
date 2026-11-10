package SharedDependencies {
    private import SoftwareItemMetadata::*;

    doc /* Shared Packages consumed by SarifMark. */

    part def SarifMarkSharedPackage {
        doc /* Shared Package: an earlier SarifMark release consumed as a CI dependency of this repository's own build pipeline. */

        metadata info : SoftwareInfo {
            description = "Shared Package: an earlier SarifMark release consumed as a CI dependency of this repository's own build pipeline.";
            designFiles = ("docs/design/shared/sarifmark.md");
            verificationFiles = ("docs/verification/shared/sarifmark.md");
            requirementFiles = ("docs/reqstream/shared/sarifmark.yaml");
        }
    }
}
