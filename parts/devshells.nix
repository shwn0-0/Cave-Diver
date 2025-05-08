{
  # config,
  pkgs,
  ...
}: {
  devshells.default = (
    args: {
      packages = with pkgs; [
        alejandra
        csharpier
        netcoredbg
        nixd
        omnisharp-roslyn
      ];
      # packagesFrom = [
      #   config.treefmt.build.devShell
      # ];
    }
  );
}
