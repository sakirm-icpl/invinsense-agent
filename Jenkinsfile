pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                script {
                    checkout scm
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    bat "dotnet build Invinsense.sln"
                }
            }
        }

    }

    post {
        success {
            archiveArtifacts artifacts: 'c:/msi/**', allowEmptyArchive: true
        }
    }
}
