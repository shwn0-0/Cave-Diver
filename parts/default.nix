{inputs, ...}: {
  imports = [
    # inputs.treefmt-nix.flakeModule
    inputs.devshell.flakeModule
  ];
  perSystem = {
    imports = [
      ./devshells.nix
      # ./treefmt.nix
    ];
  };
}
