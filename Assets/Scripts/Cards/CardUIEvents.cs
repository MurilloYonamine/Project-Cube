using System;
using System.Collections.Generic;

namespace PROJECT_CUBE.CARDS {
    public static class CardUIEvents {
        // Fired when spinner should appear/disappear. true = show, false = hide
        public static Action<bool> OnSpinnerVisibilityRequested;

        // Fired when skill UI should appear/disappear. true = show, false = hide
        public static Action<bool> OnSkillVisibilityRequested;

        // Request that UI starts the spinner visual. Parameters: available cards, selected card (final), onComplete callback
        public static Action<List<CardData>, CardData, Action> OnSpinnerStartRequested;

        // Fired when spinner UI finished hiding (fade out complete)
        public static Action OnSpinnerHiddenCompleted;
    }
}
