using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// EventArgs pour l'événement d'ajout d'un ordinateur
public sealed class OrdinateurAjouteEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurAjouteEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}
// EventArgs pour l'événement de suppression d'un ordinateur
public sealed class OrdinateurSupprimeEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}

// EventArgs pour l'événement de mise à jour d'un ordinateur

public sealed class OrdinateurMisAJourEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurMisAJourEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}