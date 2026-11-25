// Views rendered into docs/design/generated/ for the Design document.
// Per SysML2Tools syntax, 'expose' is only valid inside a named 'view' usage
// (not a 'view def' definition), and it - not 'render' - is what scopes the
// rendered diagram content.
package SarifMarkDesignViews {
    private import StandardViewDefinitions::*;
    private import SarifMark::*;

    view SoftwareStructureView : InterconnectionView {
        expose SarifMark::SarifMarkSystem;
    }

    view SarifMarkView {
        expose SarifMarkSystem;
    }

    view CliView {
        expose CliSubsystem;
    }

    view SarifView {
        expose SarifSubsystem;
    }

    view SelfTestView {
        expose SelfTestSubsystem;
    }

    view UtilitiesView {
        expose UtilitiesSubsystem;
    }

    view OtsView : GeneralView {
        expose OtsDependencies::*;
        filter not (@SysML::Documentation);
    }
}
