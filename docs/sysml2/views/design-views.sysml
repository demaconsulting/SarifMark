// Views rendered into docs/design/generated/ for the Design document.
// Reopens the SarifMark package so each 'expose' statement can reference
// its target part def directly. Per SysML2Tools syntax, 'expose' is only valid
// inside a named 'view' usage (not a 'view def' definition), and it - not
// 'render' - is what scopes the rendered diagram content.
package SarifMark {
    private import StandardViewDefinitions::*;

    view SoftwareStructureView : InterconnectionView {
        expose SarifMark::**;
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
}
